using System.Text.Json;
using DebtSnowballer.Shared.Currency;
using Microsoft.Extensions.Configuration;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class CurrencyService : ICurrencyService
{
	private readonly string _apiKey;
	private readonly string _baseUrl;
	private readonly HttpClient _httpClient;
	private readonly IGenericRepository<ExchangeRate> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CurrencyService(IUnitOfWork unitOfWork, IConfiguration configuration)
	{
		_unitOfWork = unitOfWork;
		_httpClient = new HttpClient();
		_repository = unitOfWork.ExchangeRateRepository;
		_apiKey = configuration["ExchangeRateApi:ApiKey"];
		_baseUrl = configuration["ExchangeRateApi:BaseUrl"];
	}

	public async Task<decimal> GetExchangeRate(string baseCurrency, string quoteCurrency)
	{
		ValidateCurrencyCodes(baseCurrency, quoteCurrency);

		ExchangeRate exchangeRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		if (exchangeRate == null || exchangeRate.NextUpdateTime < DateTime.UtcNow)
			await UpdateExchangeRateFromApi(baseCurrency, quoteCurrency);

		exchangeRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		return exchangeRate.Rate;
	}

	private void ValidateCurrencyCodes(string baseCurrency, string quoteCurrency)
	{
		if (!Currencies.All.Any(c => c.AlphaCode == baseCurrency) ||
		    !Currencies.All.Any(c => c.AlphaCode == quoteCurrency))
			throw new ArgumentException("Invalid currency code.");
	}


	private async Task<ExchangeRate> GetExchangeRateFromDatabase(string baseCurrency, string quoteCurrency)
	{
		return await _repository.Get(er => er.BaseCurrency == baseCurrency && er.TargetCurrency == quoteCurrency);
	}

	private async Task UpdateExchangeRateFromApi(string baseCurrency, string quoteCurrency)
	{
		try
		{
			string url = BuildApiUrl(baseCurrency);
			string apiResponse = await _httpClient.GetStringAsync(url);
			JsonDocument jsonDocument = JsonDocument.Parse(apiResponse);

			DateTime nextUpdateTime = GetNextUpdateTimeFromApiResponse(jsonDocument);

			await UpdateAllRatesForBaseCurrency(baseCurrency, jsonDocument, nextUpdateTime);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching exchange rate: {ex.Message}");
			throw;
		}
	}

	private string BuildApiUrl(string baseCurrency)
	{
		string currencies = string.Join(",", Currencies.All.Select(c => c.AlphaCode));
		return $"{_baseUrl}{_apiKey}/latest/{baseCurrency}?symbols={currencies}";
	}

	private DateTime GetNextUpdateTimeFromApiResponse(JsonDocument doc)
	{
		return DateTime.Parse(doc.RootElement.GetProperty("time_next_update_utc").GetString() ??
		                      DateTime.UtcNow.ToLongDateString());
	}

	private async Task UpdateAllRatesForBaseCurrency(string baseCurrency, JsonDocument jsonDocument,
		DateTime nextUpdateTime)
	{
		JsonElement.ObjectEnumerator rates = jsonDocument.RootElement.GetProperty("conversion_rates").EnumerateObject();
		foreach (JsonProperty rate in rates)
		{
			string targetCurrency = rate.Name;
			decimal exchangeRateValue = rate.Value.GetDecimal();

			await UpdateOrCreateExchangeRate(baseCurrency, targetCurrency, exchangeRateValue, nextUpdateTime);
		}

		await _unitOfWork.Save();
	}

	private async Task UpdateOrCreateExchangeRate(string baseCurrency, string quoteCurrency, decimal exchangeRateValue,
		DateTime nextUpdateTime)
	{
		ExchangeRate existingRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		if (existingRate == null)
		{
			existingRate = new ExchangeRate
			{
				BaseCurrency = baseCurrency,
				TargetCurrency = quoteCurrency,
				Rate = exchangeRateValue,
				NextUpdateTime = nextUpdateTime
			};
			await _repository.Insert(existingRate);
		}
		else
		{
			existingRate.Rate = exchangeRateValue;
			existingRate.NextUpdateTime = nextUpdateTime;
			_repository.Update(existingRate);
		}
	}
}
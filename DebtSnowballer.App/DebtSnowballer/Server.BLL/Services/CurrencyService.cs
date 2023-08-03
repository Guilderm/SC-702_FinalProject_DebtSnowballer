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
	private ExchangeRate _exchangeRate;

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
		_exchangeRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		if (_exchangeRate == null || _exchangeRate.NextUpdateTime < DateTime.UtcNow)
			await UpdateExchangeRateFromApi(baseCurrency);

		_exchangeRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		return _exchangeRate.ConversionRate;
	}


	private async Task<ExchangeRate> GetExchangeRateFromDatabase(string baseCurrency, string quoteCurrency)
	{
		return await _repository.Get(er => er.BaseCurrency == baseCurrency && er.QuoteCurrency == quoteCurrency);
	}

	private async Task UpdateExchangeRateFromApi(string baseCurrency)
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
		string currencies = string.Join(",", Currencies.SupportedCurrencies.Select(c => c.AlphaCode));
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
		await _repository.Delete(er => er.BaseCurrency == baseCurrency);

		JsonElement.ObjectEnumerator rates = jsonDocument.RootElement.GetProperty("conversion_rates").EnumerateObject();
		foreach (JsonProperty rate in rates)
		{
			string quoteCurrency = rate.Name;
			decimal exchangeRateValue = rate.Value.GetDecimal();

			if (Currencies.SupportedCurrencies.Any(c => c.AlphaCode == quoteCurrency))
				await CreateExchangeRate(baseCurrency, quoteCurrency, exchangeRateValue, nextUpdateTime);
		}

		await _unitOfWork.Save();
	}

	private async Task CreateExchangeRate(string baseCurrency, string quoteCurrency, decimal exchangeRateValue,
		DateTime nextUpdateTime)
	{
		ExchangeRate newRate = new()
		{
			BaseCurrency = baseCurrency,
			QuoteCurrency = quoteCurrency,
			ConversionRate = exchangeRateValue,
			NextUpdateTime = nextUpdateTime
		};
		await _repository.Insert(newRate);
	}
}
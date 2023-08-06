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
	private readonly IHttpClientFactory _clientFactory;
	private readonly IGenericRepository<ExchangeRate> _repository;
	private readonly IUnitOfWork _unitOfWork;
	private ExchangeRate _exchangeRate;

	public CurrencyService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpClientFactory clientFactory)
	{
		_unitOfWork = unitOfWork;
		_clientFactory = clientFactory;
		_apiKey = configuration["ExchangeRateApi:ApiKey"];
		_baseUrl = configuration["ExchangeRateApi:BaseUrl"];
		_repository = unitOfWork.GetRepository<ExchangeRate>();
	}

	public async Task<decimal> GetExchangeRate(string baseCurrency, string quoteCurrency)
	{
		_exchangeRate = await GetExchangeRateFromDatabase(baseCurrency, quoteCurrency);

		bool exchangeRateIsNull = _exchangeRate == null;
		bool exchangeRateIsStale = _exchangeRate != null && _exchangeRate.NextUpdateTime < DateTime.UtcNow;

		if (exchangeRateIsNull || exchangeRateIsStale)
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
			var client = _clientFactory.CreateClient();
			string apiResponse = await client.GetStringAsync($"{_baseUrl}{_apiKey}/latest/{baseCurrency}");
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

	private static DateTime GetNextUpdateTimeFromApiResponse(JsonDocument doc)
	{
		DateTime time = doc.RootElement.TryGetProperty("time_next_update_utc", out JsonElement timeNextUpdateUtc) &&
		                timeNextUpdateUtc.GetString() != null
			? DateTime.Parse(timeNextUpdateUtc.GetString())
			: DateTime.UtcNow.AddHours(12);

		return time;
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
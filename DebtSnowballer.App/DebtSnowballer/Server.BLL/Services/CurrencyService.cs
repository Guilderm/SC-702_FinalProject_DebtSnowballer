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
	private readonly IConfiguration _configuration;
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

	public async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
	{
		if (!Enum.IsDefined(typeof(Currencies), fromCurrency) ||
		    !Enum.IsDefined(typeof(Currencies), toCurrency))
			throw new ArgumentException("Invalid currency code.");

		// Try to get the exchange rate from the database
		ExchangeRate exchangeRate =
			await _repository.Get(er => er.BaseCurrency == fromCurrency && er.TargetCurrency == toCurrency);

		// If the exchange rate is not in the database or it's older than 12 hours, fetch it from the API
		if (exchangeRate == null || exchangeRate.LastUpdated < DateTime.UtcNow.AddHours(-12))
			try
			{
				// Join the allowed currencies into a comma-separated string
				string currencies = string.Join(",", Currencies.All);

				// Append the currencies to the URL
				string url = $"{_baseUrl}{_apiKey}/latest/{fromCurrency}?symbols={currencies}";

				string json = await _httpClient.GetStringAsync(url);
				JsonDocument doc = JsonDocument.Parse(json);

				// Update all rates for the base currency
				JsonElement.ObjectEnumerator rates = doc.RootElement.GetProperty("conversion_rates").EnumerateObject();
				foreach (JsonProperty rate in rates)
				{
					string targetCurrency = rate.Name;
					decimal exchangeRateValue = rate.Value.GetDecimal();

					ExchangeRate existingRate = await _repository.Get(er =>
						er.BaseCurrency == fromCurrency && er.TargetCurrency == targetCurrency);
					if (existingRate == null)
					{
						// If the exchange rate was not in the database, create a new record
						existingRate = new ExchangeRate
						{
							BaseCurrency = fromCurrency,
							TargetCurrency = targetCurrency,
							Rate = exchangeRateValue,
							LastUpdated = DateTime.UtcNow
						};
						await _repository.Insert(existingRate);
					}
					else
					{
						// If the exchange rate was in the database but was old, update the existing record
						existingRate.Rate = exchangeRateValue;
						existingRate.LastUpdated = DateTime.UtcNow;
						_repository.Update(existingRate);
					}
				}

				await _unitOfWork.Save();

				// Get the exchange rate for the target currency
				exchangeRate =
					await _repository.Get(er => er.BaseCurrency == fromCurrency && er.TargetCurrency == toCurrency);
			}
			catch (Exception ex)
			{
				// Log the error and rethrow the exception
				// You might want to use a logging framework like Serilog or NLog here
				Console.WriteLine($"Error fetching exchange rate: {ex.Message}");
				throw;
			}

		return exchangeRate.Rate;
	}
}
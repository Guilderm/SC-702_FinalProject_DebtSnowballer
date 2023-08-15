using System.Text.Json;
using AutoMapper;
using DebtSnowballer.Shared.Currency;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class ExchangeRateManagement
{
	private readonly string _apiKey;
	private readonly string _baseUrl;
	private readonly IHttpClientFactory _clientFactory;
	private readonly IMapper _mapper;
	private readonly IGenericRepository<ExchangeRate> _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserPreferenceManagement _userPreferenceManagement;
	private ExchangeRate _exchangeRate;

	public ExchangeRateManagement(IUnitOfWork unitOfWork, IConfiguration configuration,
		IHttpClientFactory clientFactory,
		IMapper mapper, UserPreferenceManagement userProfileManagement)
	{
		_unitOfWork = unitOfWork;
		_clientFactory = clientFactory;
		_mapper = mapper;
		_apiKey = configuration["ExchangeRateApi:ApiKey"];
		_baseUrl = configuration["ExchangeRateApi:BaseUrl"];
		_repository = unitOfWork.GetRepository<ExchangeRate>();
		_userPreferenceManagement = userProfileManagement;
	}

	public async Task<IEnumerable<ExchangeRateDto>> GetUsersExchangeRates(string auth0UserId)
	{
		UserPreferenceDto userPreference = await _userPreferenceManagement.GetUserPreference(auth0UserId);
		IEnumerable<ExchangeRate> usersExchangeRates =
			await GetAndUpdateExchangeRatesForBaseCurrency(userPreference.BaseCurrency);

		return _mapper.Map<IEnumerable<ExchangeRateDto>>(usersExchangeRates);
	}

	private async Task<IEnumerable<ExchangeRate>> GetAndUpdateExchangeRatesForBaseCurrency(string baseCurrency)
	{
		_exchangeRate = await GetExchangeRateFromDatabase(baseCurrency);

		bool isExchangeRateNull = _exchangeRate == null;
		bool isExchangeRateStale = _exchangeRate != null && _exchangeRate.NextUpdateTime < DateTime.UtcNow;

		if (isExchangeRateNull || isExchangeRateStale)
			await UpdateExchangeRateFromApi(baseCurrency);

		IEnumerable<ExchangeRate> exchangeRateList = await GetAllExchangeRatesFromDatabase(baseCurrency);

		return exchangeRateList;
	}

	private async Task<ExchangeRate> GetExchangeRateFromDatabase(string baseCurrency)
	{
		return await _repository.Get(er => er.BaseCurrency == baseCurrency);
	}

	private async Task<IEnumerable<ExchangeRate>> GetAllExchangeRatesFromDatabase(string baseCurrency)
	{
		return await _repository.GetAll(er => er.BaseCurrency == baseCurrency);
	}

	private async Task UpdateExchangeRateFromApi(string baseCurrency)
	{
		try
		{
			HttpClient client = _clientFactory.CreateClient();
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
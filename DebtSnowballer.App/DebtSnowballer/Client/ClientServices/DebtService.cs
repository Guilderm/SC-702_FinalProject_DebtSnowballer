using System.Net.Http.Json;
using System.Security.Claims;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientServices;

public class DebtService : IDebtService
{
	private readonly string _backendUrl;
	private readonly HttpClient _httpClient;
	private readonly ILogger<DebtService> _logger;

	public DebtService(HttpClient httpClient, ILogger<DebtService> logger)
	{
		_httpClient = httpClient;
		_backendUrl = _httpClient.BaseAddress + "api/Debt";
		_logger = logger;
	}

	public async Task DeleteDebt(int id)
	{
		_logger.LogInformation("Deleting debt with id {id}", id);
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
		_logger.LogInformation("Deleted debt with id {id}", id);
	}

	public async Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency()
	{
		_logger.LogInformation("Fetching all debts in quote currency");
		IList<DebtDto> debts =
			await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_backendUrl}/GetAllDebtsInQuoteCurrency");
		_logger.LogInformation("Fetched {count} debts in quote currency", debts.Count);
		return debts;
	}

	public async Task<DebtDto> GetDebtById(int id)
	{
		_logger.LogInformation("Fetching debt with id {id}", id);
		DebtDto debt = await _httpClient.GetFromJsonAsync<DebtDto>($"{_backendUrl}/{id}");
		_logger.LogInformation("Fetched debt with id {id}", id);
		return debt;
	}

	public async Task<DebtDto> AddDebt(DebtDto debtDto)
	{
		_logger.LogInformation("Adding new debt");
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		_logger.LogInformation("Added new debt with id {id}", result.Id);
		return result;
	}

	public async Task<DebtDto> UpdateDebt(DebtDto debtDto)
	{
		_logger.LogInformation("Updating debt with id {id}", debtDto.Id);
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_backendUrl}/{debtDto.Id}", debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		_logger.LogInformation("Updated debt with id {id}", result.Id);
		return result;
	}

	public async Task<IList<ExchangeRateDto>> GetUsersExchangeRates(ClaimsPrincipal userId)
	{
		_logger.LogInformation("Fetching exchange rates for user {userId}", userId);
		IList<ExchangeRateDto> exchangeRates =
			await _httpClient.GetFromJsonAsync<IList<ExchangeRateDto>>($"{_backendUrl}/GetUsersExchangeRates");
		_logger.LogInformation("Fetched {count} exchange rates for user {userId}", exchangeRates.Count, userId);
		return exchangeRates;
	}
}
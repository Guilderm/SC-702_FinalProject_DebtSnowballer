using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

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

	public async Task DeleteLoan(int id)
	{
		_logger.LogInformation("Deleting debt with id {id}", id);
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
		_logger.LogInformation("Deleted debt with id {id}", id);
	}

	public async Task<IList<LoanDetailDto>> GetAllDebtsInQuoteCurrency()
	{
		_logger.LogInformation("Fetching all debts in quote currency");
		IList<LoanDetailDto> debts =
			await _httpClient.GetFromJsonAsync<IList<LoanDetailDto>>($"{_backendUrl}/GetAllDebtsInQuoteCurrency");
		_logger.LogInformation("Fetched {count} debts in quote currency", debts.Count);
		return debts;
	}

	public async Task<LoanDetailDto> GetDebtById(int id)
	{
		_logger.LogInformation("Fetching debt with id {id}", id);
		LoanDetailDto loanDetail = await _httpClient.GetFromJsonAsync<LoanDetailDto>($"{_backendUrl}/{id}");
		_logger.LogInformation("Fetched debt with id {id}", id);
		return loanDetail;
	}

	public async Task<LoanDetailDto> AddDebt(LoanDetailDto loanDetailDto)
	{
		_logger.LogInformation("Adding new debt");
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, loanDetailDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		LoanDetailDto result = await response.Content.ReadFromJsonAsync<LoanDetailDto>();
		_logger.LogInformation("Added new debt with id {id}", result.Id);
		return result;
	}

	public async Task<LoanDetailDto> UpdateDebt(LoanDetailDto loanDetailDto)
	{
		_logger.LogInformation("Updating debt with id {id}", loanDetailDto.Id);
		HttpResponseMessage response =
			await _httpClient.PutAsJsonAsync($"{_backendUrl}/{loanDetailDto.Id}", loanDetailDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		LoanDetailDto result = await response.Content.ReadFromJsonAsync<LoanDetailDto>();
		_logger.LogInformation("Updated debt with id {id}", result.Id);
		return result;
	}

	public async Task<IList<ExchangeRateDto>> GetUsersExchangeRates()
	{
		_logger.LogInformation("Fetching exchange rates ");
		IList<ExchangeRateDto> exchangeRates =
			await _httpClient.GetFromJsonAsync<IList<ExchangeRateDto>>($"{_backendUrl}/GetUsersExchangeRates");
		_logger.LogInformation("Fetched {count} exchange rates", exchangeRates.Count);
		return exchangeRates;
	}
}
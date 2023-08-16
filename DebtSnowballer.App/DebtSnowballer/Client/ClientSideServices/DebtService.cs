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
        _logger.LogInformation($"Deleting loan with ID: {id}");
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
        _logger.LogInformation("Deleted debt with id {id}", id);
    }

    public async Task<IList<LoanDto>> GetAllDebtsInQuoteCurrency()
    {
        _logger.LogInformation("Fetching all debts in quote currency");
        IList<LoanDto> debts =
            await _httpClient.GetFromJsonAsync<IList<LoanDto>>($"{_backendUrl}/GetAllDebtsInQuoteCurrency");
        _logger.LogInformation("Fetched {count} debts in quote currency", debts.Count);
        return debts;
    }

    public async Task<LoanDto> CreateLoan(LoanDto loan)
    {
        _logger.LogInformation($"Adding new loan with ID: {loan.Id} Name: {loan.Name}");
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, loan);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error posting debt: {response.ReasonPhrase}");
        LoanDto result = await response.Content.ReadFromJsonAsync<LoanDto>();
        _logger.LogInformation($"Added new loan with ID: {loan.Id} Name: {loan.Name}");
        return result;
    }

    public async Task<LoanDto> UpdateLoan(LoanDto loanDto)
    {
        _logger.LogInformation("Updating debt with id {id}", loanDto.Id);
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync($"{_backendUrl}/{loanDto.Id}", loanDto);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error updating debt: {response.ReasonPhrase}");
        LoanDto result = await response.Content.ReadFromJsonAsync<LoanDto>();
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
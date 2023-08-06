using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class DebtService : IDebtService
{
	private readonly string _backendUrl;
	private readonly HttpClient _httpClient;

	public DebtService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_backendUrl = _httpClient.BaseAddress + "api/Debt";
	}

	public async Task DeleteDebt(int id)
	{
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
	}

	public async Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency()
	{
		IList<DebtDto> debts =
			await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_backendUrl}/GetAllDebtsInQuoteCurrency");
		return debts;
	}

	public async Task<DebtDto> GetDebtById(int id)
	{
		DebtDto debt = await _httpClient.GetFromJsonAsync<DebtDto>($"{_backendUrl}/{id}");
		return debt;
	}

	public async Task<DebtDto> AddDebt(DebtDto debtDto)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}

	public async Task<DebtDto> UpdateDebt(DebtDto debtDto)
	{
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_backendUrl}/{debtDto.Id}", debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}

	public async Task<IList<ExchangeRateDto>> GetUsersExchangeRates(string userId)
	{
		IList<ExchangeRateDto> exchangeRates =
			await _httpClient.GetFromJsonAsync<IList<ExchangeRateDto>>(
				$"{_backendUrl}/GetUsersExchangeRates?userId={userId}");
		return exchangeRates;
	}
}
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
		//_backendUrl = _httpClient.BaseAddress + "api/Debt";
	}

	public async Task DeleteDebt(int id)
	{
		Console.WriteLine($"Entering DeleteDebt with id: {id}");
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
		Console.WriteLine($"Exiting DeleteDebt with id: {id}");
	}

	public async Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency()
	{
		Console.WriteLine("Entering GetAllDebtsInQuoteCurrency");
		IList<DebtDto> debts = await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_backendUrl}");
		Console.WriteLine($"Exiting GetAllDebtsInQuoteCurrency with {debts.Count} debts");
		return debts;
	}

	public async Task<DebtDto> GetDebtById(int id)
	{
		Console.WriteLine($"Entering GetDebtById with id: {id}");
		DebtDto debt = await _httpClient.GetFromJsonAsync<DebtDto>($"{_backendUrl}/{id}");
		Console.WriteLine($"Exiting GetDebtById with id: {id}");
		return debt;
	}

	public async Task<DebtDto> AddDebt(DebtDto debtDto)
	{
		Console.WriteLine("Entering AddDebt");
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		Console.WriteLine("Exiting AddDebt");
		return result;
	}

	public async Task<DebtDto> UpdateDebt(DebtDto debtDto)
	{
		Console.WriteLine($"Entering UpdateDebt with id: {debtDto.Id}");
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_backendUrl}/{debtDto.Id}", debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		Console.WriteLine($"Exiting UpdateDebt with id: {debtDto.Id}");
		return result;
	}

	public async Task<IList<DebtDto>> GetAllDebtsInBaseCurrency()
	{
		Console.WriteLine("Entering GetAllDebtsInBaseCurrency");
		IList<DebtDto> debts =
			await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_backendUrl}/GetAllDebtsInBaseCurrency");
		Console.WriteLine($"Exiting GetAllDebtsInBaseCurrency with {debts.Count} debts");
		return debts;
	}
}
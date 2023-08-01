using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class DebtService : IDebtService
{
	private readonly string _apiurl;
	private readonly HttpClient _httpClient;

	public DebtService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiurl = configuration["ApiEndpoint:Url"] + "/Debt";
	}

	public async Task DeleteDebt(int id)
	{
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiurl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting debt: {response.ReasonPhrase}");
	}

	public async Task<IList<DebtDto>> GetDebts()
	{
		IList<DebtDto> debts = await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_apiurl}");
		return debts;
	}

	public async Task<DebtDto> GetDebtById(int id)
	{
		DebtDto debt = await _httpClient.GetFromJsonAsync<DebtDto>($"{_apiurl}/{id}");
		return debt;
	}

	public async Task<DebtDto> AddDebt(DebtDto debtDto)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_apiurl, debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}

	public async Task<DebtDto> UpdateDebt(DebtDto debtDto)
	{
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_apiurl}/{debtDto.Id}", debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}

	public async Task<IList<DebtDto>> GetAllDebtsInBaseCurrency()
	{
		IList<DebtDto> debts =
			await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_apiurl}/GetAllDebtsInBaseCurrency");
		return debts;
	}
}
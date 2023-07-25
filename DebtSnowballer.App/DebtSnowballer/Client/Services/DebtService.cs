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

	public async Task DeleteItem(int id)
	{
		Console.WriteLine($"Sending DELETE request to {_apiurl}/{id}");
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiurl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting item: {response.ReasonPhrase}");
	}

	public async Task<IList<DebtDto>> GetDebtbyAuth0UserId(string auth0UserId)
	{
		Console.WriteLine($"Entered GetDebtbyAuth0UserId, will send GET request to {_apiurl}/{auth0UserId}");
		IList<DebtDto> debts = await _httpClient.GetFromJsonAsync<IList<DebtDto>>($"{_apiurl}/{auth0UserId}");
		return debts;
	}

	public async Task<DebtDto> GetDebtByIdAndAuth0UserId(int id, string auth0UserId)
	{
		Console.WriteLine($"Entered GetDebtByIdAndAuth0UserId, will send GET request to {_apiurl}/{id}/{auth0UserId}");
		DebtDto debt = await _httpClient.GetFromJsonAsync<DebtDto>($"{_apiurl}/{id}/{auth0UserId}");
		return debt;
	}

	public async Task<DebtDto> AddItem(DebtDto debtDto)
	{
		Console.WriteLine($"Sending POST request to {_apiurl}");
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_apiurl, debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error posting debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}

	public async Task<DebtDto> UpdateItem(DebtDto debtDto)
	{
		Console.WriteLine($"Sending PUT request to {_apiurl}/{debtDto.Id}");
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_apiurl}/{debtDto.Id}", debtDto);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating debt: {response.ReasonPhrase}");
		DebtDto result = await response.Content.ReadFromJsonAsync<DebtDto>();
		return result;
	}
}
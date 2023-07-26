using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class StrategyService : IStrategyService
{
	private readonly string _apiurl;
	private readonly HttpClient _httpClient;

	public StrategyService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiurl = configuration["ApiEndpoint:Url"] + "/Strategy";
	}

	public async Task DeleteStrategy(int id)
	{
		HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiurl}/{id}");
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error deleting strategy: {response.ReasonPhrase}");
	}

	
}
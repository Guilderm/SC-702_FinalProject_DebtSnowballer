using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace Server.BLL.ServerServices;

public class MultiPurposeService : IMultiPurposeService
{
	private readonly HttpClient _httpClient;

	public MultiPurposeService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<StrategyTypeDto>> GetAllStrategyTypesAsync()
	{
		return await _httpClient.GetFromJsonAsync<List<StrategyTypeDto>>("api/MultiPurpose");
	}
}
using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public class MultiPurposeService : IMultiPurposeService
{
	private readonly string _backendUrl;
	private readonly HttpClient _httpClient;
	private readonly ILogger<MultiPurposeService> _logger;

	public MultiPurposeService(HttpClient httpClient, ILogger<MultiPurposeService> logger)
	{
		_httpClient = httpClient;
		_backendUrl = _httpClient.BaseAddress + "api/MultiPurpose/";
		_logger = logger;
	}

	public async Task<IList<DebtPayDownMethodDto>> GetAllStrategyTypes()
	{
		_logger.LogInformation("Fetching all strategy types");

		HttpResponseMessage response = await _httpClient.GetAsync($"{_backendUrl}GetAllStrategyTypes");

		if (response.IsSuccessStatusCode)
		{
			IList<DebtPayDownMethodDto> strategyTypes =
				await response.Content.ReadFromJsonAsync<IList<DebtPayDownMethodDto>>();
			_logger.LogInformation("Fetched {count} strategy types", strategyTypes?.Count);
			return strategyTypes;
		}

		_logger.LogError("Error fetching strategy types: {reason}", response.ReasonPhrase);
		throw new Exception($"Error fetching strategy types: {response.ReasonPhrase}");
	}
}
using System.Text.Json;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace DebtSnowballer.Client.Services;

public class DebtService : GenericService<DebtDto>, IDebtService
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;

	public DebtService(HttpClient httpClient, IConfiguration configuration,
		AuthenticationStateProvider authenticationStateProvider)
		: base(httpClient, configuration["ApiEndpoint:Url"] + "Debt")
	{
		_authenticationStateProvider = authenticationStateProvider;
	}

	public async Task<IList<DebtDto>> GetDebtbySUD(string Auth0SUD)
	{
		var response = await _httpClient.GetAsync($"{_APIURL}/{Auth0SUD}");
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<IList<DebtDto>>(content);
	}
}
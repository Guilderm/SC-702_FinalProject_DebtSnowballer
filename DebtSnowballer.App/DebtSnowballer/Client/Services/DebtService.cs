using System.Text.Json;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DebtSnowballer.Client.Services
{
	public class DebtService : IDebtService
	{
		private readonly HttpClient _httpClient;
		private readonly string _APIURL;
		private readonly AuthenticationStateProvider _authenticationStateProvider;

		public async Task<DebtDto> GetItem(int id)
		{
			var response = await _httpClient.GetAsync($"{_APIURL}/{id}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<DebtDto>(content);
		}

		public async Task DeleteItem(int id)
		{
			var response = await _httpClient.DeleteAsync($"{_APIURL}/{id}");
			response.EnsureSuccessStatusCode();
		}

		public DebtService(HttpClient httpClient, IConfiguration configuration,
			AuthenticationStateProvider authenticationStateProvider)
		{
			_httpClient = httpClient;
			_APIURL = configuration["ApiEndpoint:Url"] + "/Debt";
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<IList<DebtDto>> GetDebtbyAuth0UserId(string auth0UserId)
		{
			var response = await _httpClient.GetAsync($"{_APIURL}/{auth0UserId}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<IList<DebtDto>>(content);
		}

		public async Task<DebtDto> GetDebtByIdAndAuth0UserId(int id, string auth0UserId)
		{
			var response = await _httpClient.GetAsync($"{_APIURL}/{id}/{auth0UserId}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<DebtDto>(content);
		}
	}


	}
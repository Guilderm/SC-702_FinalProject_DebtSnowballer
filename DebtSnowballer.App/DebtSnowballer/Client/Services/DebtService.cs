using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace DebtSnowballer.Client.Services
	{
	public class DebtService : IDebtService
		{
		private readonly string _APIURL;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly HttpClient _httpClient;

		public DebtService(HttpClient httpClient, IConfiguration configuration,
			AuthenticationStateProvider authenticationStateProvider)
			{
			_httpClient = httpClient;
			_APIURL = configuration["ApiEndpoint:Url"] + "/Debt";
			_authenticationStateProvider = authenticationStateProvider;
			}

		public async Task DeleteItem(int id)
			{
			Console.WriteLine($"Sending DELETE request to {_APIURL}/{id}");
			HttpResponseMessage response = await _httpClient.DeleteAsync($"{_APIURL}/{id}");
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Error deleting item: {response.ReasonPhrase}");
			}

		public async Task<IList<DebtDto>> GetDebtbyAuth0UserId(string auth0UserId)
			{
			Console.WriteLine($"Entered GetDebtbyAuth0UserId, will send GET request to {_APIURL}/{auth0UserId}");
			HttpResponseMessage response = await _httpClient.GetAsync($"{_APIURL}/{auth0UserId}");
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Error getting debts: {response.ReasonPhrase}");
			string content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<IList<DebtDto>>(content,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			}

		public async Task<DebtDto> GetDebtByIdAndAuth0UserId(int id, string auth0UserId)
			{
			Console.WriteLine($"Entered GetDebtByIdAndAuth0UserId, will send GET request to {_APIURL}/{id}/{auth0UserId}");
			HttpResponseMessage response = await _httpClient.GetAsync($"{_APIURL}/{id}/{auth0UserId}");
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Error getting debt: {response.ReasonPhrase}");
			string content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<DebtDto>(content,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			}

		public async Task<DebtDto> PostDebt(DebtDto debtDto)
			{
			var content = new StringContent(JsonSerializer.Serialize(debtDto), Encoding.UTF8, "application/json");
			Console.WriteLine($"Sending POST request to {_APIURL}");
			HttpResponseMessage response = await _httpClient.PostAsync(_APIURL, content);
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Error posting debt: {response.ReasonPhrase}");
			string result = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<DebtDto>(result,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			}
		}
	}

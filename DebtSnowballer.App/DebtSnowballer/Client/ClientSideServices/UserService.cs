using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public class UserService : IUserService
{
	private readonly string _backendUrl;
	private readonly HttpClient _httpClient;

	public UserService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_backendUrl = _httpClient.BaseAddress + "api/User";
	}

	public async Task<UserPreferenceDto> GetUserPreferenceAsync()
	{
		return await _httpClient.GetFromJsonAsync<UserPreferenceDto>(_backendUrl + "/GetUserPreference");
	}

	public async Task<UserPreferenceDto> UpdateUserPreferenceAsync(UserPreferenceDto userPreferenceDto)
	{
		HttpResponseMessage response =
			await _httpClient.PutAsJsonAsync(_backendUrl + "/UpdateUserPreference", userPreferenceDto);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<UserPreferenceDto>();
	}

	public async Task<UserProfileDto> GetUserProfileAsync()
	{
		return await _httpClient.GetFromJsonAsync<UserProfileDto>(_backendUrl + "/GetUserProfile");
	}
}
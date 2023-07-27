using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class UserProfileService : IUserProfileService
{
	private readonly HttpClient _httpClient;
	private readonly string _apiurl;

	public UserProfileService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiurl = configuration["ApiEndpoint:Url"] + "/UserProfile";
	}

	public async Task UpdateBaseCurrency(string baseCurrency)
	{
		UserProfileDto userProfile = await GetUserProfile();
		userProfile.BaseCurrency = baseCurrency;
		await UpdateUserProfile(userProfile);
	}

	public async Task<UserProfileDto> GetUserProfile()
	{
		UserProfileDto userProfile = await _httpClient.GetFromJsonAsync<UserProfileDto>($"{_apiurl}");
		return userProfile;
	}

	private async Task UpdateUserProfile(UserProfileDto userProfile)
	{
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_apiurl}/{userProfile}", userProfile);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error updating user profile: {response.ReasonPhrase}");
	}
}

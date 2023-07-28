using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class UserProfileService : IUserProfileService
{
	private readonly string _apiurl;
	private readonly HttpClient _httpClient;

	public UserProfileService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiurl = configuration["ApiEndpoint:Url"] + "/UserProfile";
	}

	public async Task<UserProfileDto> GetUserProfile(ClaimsPrincipal user)
	{
		UserProfileDto rawUserProfile = await CreateUserProfileFromClaimsAsync(user);
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_apiurl}", rawUserProfile);

		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error validating user profile: {response.ReasonPhrase}");

		UserProfileDto validatedUserProfile = await response.Content.ReadFromJsonAsync<UserProfileDto>();

		return validatedUserProfile;
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

	private async Task<UserProfileDto> CreateUserProfileFromClaimsAsync(ClaimsPrincipal user)
	{
		foreach (Claim claim in user.Claims) Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");

		DateTime.TryParse(user.Claims.FirstOrDefault(c => c.Type == "updated_at")?.Value, out DateTime createdAt);

		UserProfileDto rawProfileDto = new()
		{
			Auth0UserId = user.Claims.FirstOrDefault(c =>
				c.Type == "sub")?.Value,
			GivenName = user.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
			FamilyName = user.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
			NickName = user.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value,
			FullName = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
			Picture = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
			Locale = user.Claims.FirstOrDefault(c => c.Type == "Locale")?.Value,
			CreatedAt = createdAt
		};

		// Log the contents of rawProfileDto to the console
		string rawProfileDtoJson = JsonSerializer.Serialize(rawProfileDto);
		Console.WriteLine($"RawProfileDto: {rawProfileDtoJson}");

		return rawProfileDto;
	}
}
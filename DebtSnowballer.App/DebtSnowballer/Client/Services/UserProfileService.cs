using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class UserProfileService : IUserProfileService
{
	private readonly string _backendUrl;
	private readonly HttpClient _httpClient;

	public UserProfileService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_backendUrl = _httpClient.BaseAddress + "api/UserProfile";
	}

	public async Task<UserProfileDto> CreateUpdateUserProfile(ClaimsPrincipal user)
	{
		Console.WriteLine("Entered function 'CreateUpdateUserProfile'");
		UserProfileDto rawUserProfile = CreateUserProfileFromClaims(user);
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_backendUrl}", rawUserProfile);

		if (!response.IsSuccessStatusCode)
			throw new Exception($"Error validating user profile: {response.ReasonPhrase}");

		UserProfileDto validatedUserProfile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
		Console.WriteLine($"Successfully validated user profile: {JsonSerializer.Serialize(validatedUserProfile)}");
		return validatedUserProfile;
	}

	public async Task UpdateBaseCurrency(string baseCurrency)
	{
		Console.WriteLine($"Entered function 'UpdateBaseCurrency' with input: {baseCurrency}");
		HttpRequestMessage request = new(HttpMethod.Put, $"{_backendUrl}/UpdateBaseCurrency/{baseCurrency}");
		HttpResponseMessage response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Error updating user profile: {response.ReasonPhrase}");
			throw new Exception($"Error updating user profile: {response.ReasonPhrase}");
		}

		Console.WriteLine($"Successfully updated base currency to: {baseCurrency}");
	}

	public async Task UpdatePreferredMonthlyPayment(decimal preferredMonthlyPayment)
	{
		Console.WriteLine($"Entered function 'UpdatePreferredMonthlyPayment' with input: {preferredMonthlyPayment}");
		HttpRequestMessage request = new(HttpMethod.Put, $"{_backendUrl}/UpdateBaseCurrency/{preferredMonthlyPayment}");
		HttpResponseMessage response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Error updating user profile: {response.ReasonPhrase}");
			throw new Exception($"Error updating user profile: {response.ReasonPhrase}");
		}

		Console.WriteLine($"Successfully updated base currency to: {preferredMonthlyPayment}");
	}

	private UserProfileDto CreateUserProfileFromClaims(ClaimsPrincipal user)
	{
		DateTime.TryParse(user.Claims.FirstOrDefault(c => c.Type == "updated_at")?.Value, out DateTime createdAt);

		UserProfileDto rawProfileDto = new()
		{
			Auth0UserId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
			GivenName = user.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
			FamilyName = user.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
			NickName = user.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value,
			FullName = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
			Picture = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
			Locale = user.Claims.FirstOrDefault(c => c.Type == "Locale")?.Value,
			CreatedAt = createdAt
		};

		string rawProfileDtoJson = JsonSerializer.Serialize(rawProfileDto);
		Console.WriteLine($"RawProfileDto: {rawProfileDtoJson}");

		return rawProfileDto;
	}
}
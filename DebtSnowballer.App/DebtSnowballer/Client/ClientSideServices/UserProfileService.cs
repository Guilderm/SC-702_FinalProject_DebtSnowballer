using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

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

	public async Task UpdateDebtPlanMonthlyPayment(decimal debtPlanMonthlyPayment)
	{
		Console.WriteLine($"Entered function 'UpdateDebtPlanMonthlyPayment' with input: {debtPlanMonthlyPayment}");
		HttpRequestMessage request = new(HttpMethod.Put, $"{_backendUrl}/UpdateBaseCurrency/{debtPlanMonthlyPayment}");
		HttpResponseMessage response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Error updating user profile: {response.ReasonPhrase}");
			throw new Exception($"Error updating user profile: {response.ReasonPhrase}");
		}

		Console.WriteLine($"Successfully updated base currency to: {debtPlanMonthlyPayment}");
	}

	public async Task<decimal> GetDebtPlanMonthlyPayment()
	{
		Console.WriteLine("Entered function 'GetDebtPlanMonthlyPayment'");
		HttpResponseMessage response = await _httpClient.GetAsync($"{_backendUrl}/GetDebtPlanMonthlyPayment");

		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Error fetching DebtPlanMonthlyPayment: {response.ReasonPhrase}");
			throw new Exception($"Error fetching DebtPlanMonthlyPayment: {response.ReasonPhrase}");
		}

		decimal debtPlanMonthlyPayment = await response.Content.ReadFromJsonAsync<decimal>();
		Console.WriteLine($"Successfully fetched DebtPlanMonthlyPayment: {debtPlanMonthlyPayment}");
		return debtPlanMonthlyPayment;
	}

	public async Task<UserProfileDto> UpdateSelectedStrategy(int strategyTypeId)
	{
		Console.WriteLine($"Entered function 'UpdateSelectedStrategy' with input: {strategyTypeId}");
		HttpRequestMessage request = new(HttpMethod.Patch, $"{_backendUrl}/PatchSelectedStrategy/{strategyTypeId}");
		HttpResponseMessage response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Error updating user profile: {response.ReasonPhrase}");
			throw new Exception($"Error updating user profile: {response.ReasonPhrase}");
		}

		UserProfileDto updatedUserProfile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
		Console.WriteLine($"Successfully updated strategy type to: {strategyTypeId}");
		return updatedUserProfile;
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
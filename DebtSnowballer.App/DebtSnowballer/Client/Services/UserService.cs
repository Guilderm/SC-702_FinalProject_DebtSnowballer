using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace DebtSnowballer.Client.Services;

public class UserService : IUserService
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;

	public UserService(AuthenticationStateProvider authenticationStateProvider)
	{
		_authenticationStateProvider = authenticationStateProvider;
	}

	public async Task<string> GetUserSud()
	{
		AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		if (!authState.User.Identity.IsAuthenticated)
		{
			Console.WriteLine("User is not authenticated.");
			throw new Exception("User is not authenticated.");
		}

		// Log each claim
		foreach (var claim in authState.User.Claims)
			Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");

		Console.WriteLine($"IsAuthenticated: {authState.User.Identity.IsAuthenticated}");
		Claim userIdClaim = authState.User.Claims.FirstOrDefault(c => c.Type == "sid");
		if (userIdClaim == null)
		{
			Console.WriteLine("User SID claim is null.");
			throw new Exception("User SID claim is null.");
		}

		Console.WriteLine($"User SID claim: {userIdClaim.Value}");
		return userIdClaim.Value;
	}
}
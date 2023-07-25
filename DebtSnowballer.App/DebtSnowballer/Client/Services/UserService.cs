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

	public async Task<string> GetUserSUD()
	{
		AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		Console.WriteLine($"IsAuthenticated: {authState.User.Identity.IsAuthenticated}");
		foreach (var claim in authState.User.Claims)
		{
			Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
		}
		Claim userIdClaim = authState.User.Claims.FirstOrDefault(c => c.Type == "sid");
		Console.WriteLine($"userIdClaim: {userIdClaim}");
		return userIdClaim?.Value;
	}}
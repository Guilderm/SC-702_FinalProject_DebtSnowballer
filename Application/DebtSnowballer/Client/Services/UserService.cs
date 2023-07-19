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
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var userIdClaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		return userIdClaim?.Value;
	}
}
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerSideServices;

namespace Server.UIL.Controllers;

public class UserController : BaseController
{
	private readonly ILogger<UserController> _logger;
	private readonly UserPreferenceManagement _userPreferenceManagement;
	private readonly UserProfileManagement _userProfileManagement;

	public UserController(UserPreferenceManagement userPreferenceManagement, ILogger<UserController> logger,
		UserProfileManagement userProfileManagement)
		: base(logger)
	{
		_logger = logger;
		_userPreferenceManagement = userPreferenceManagement;
		_userProfileManagement = userProfileManagement;
	}


	[HttpGet("GetUserPreference")]
	public async Task<IActionResult> GetUserPreference()
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Fetching user preference for user {userId}", auth0UserId);

		UserPreferenceDto userPreference = await _userPreferenceManagement.GetUserPreference(auth0UserId);

		_logger.LogInformation("Fetched user preference for user {userId}", auth0UserId);

		return Ok(userPreference);
	}

	[HttpPut("UpdateUserPreference")]
	public async Task<IActionResult> PutUserPreference([FromBody] UserPreferenceDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Updating user preference for user {userId}", auth0UserId);

		UserPreferenceDto updatedUserPreference =
			await _userPreferenceManagement.UpdateUserPreference(requestDto, auth0UserId);

		_logger.LogInformation("User preference updated for user {userId}", auth0UserId);

		return Ok(updatedUserPreference);
	}

	[HttpGet("GetUserProfile")]
	public async Task<IActionResult> GetUserProfile()
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Fetching user profile for user {userId}", auth0UserId);

		UserProfileDto userProfile = await _userProfileManagement.GetUserProfile(auth0UserId);

		_logger.LogInformation("Fetched user profile for user {userId}", auth0UserId);

		return Ok(userProfile);
	}
}
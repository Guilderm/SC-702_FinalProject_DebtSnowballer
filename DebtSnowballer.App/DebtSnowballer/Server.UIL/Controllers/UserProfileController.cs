using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
	private readonly UserProfileManagement _userProfileManagement;
	private readonly ILogger<UserProfileController> _logger;

	public UserProfileController(UserProfileManagement userProfileManagement, ILogger<UserProfileController> logger)
	{
		_userProfileManagement = userProfileManagement;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var auth0UserId = GetAuth0UserId();

		UserProfileDto userProfile = await _userProfileManagement.GetUserProfile();

		return Ok(userProfile);
	}

	[HttpPut]
	public async Task<IActionResult> Put([FromBody] UserProfileDto userProfileDto)
	{
		var auth0UserId = GetAuth0UserId();

		await _userProfileManagement.UpdateUserProfile(userProfileDto, auth0UserId);

		return NoContent();
	}

	private string GetAuth0UserId()
	{
		var auth0UserId = User.Claims
			.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
			?.Value;

		if (string.IsNullOrEmpty(auth0UserId))
			_logger.LogError("Auth0 User ID claim is null or empty.");

		return auth0UserId;
	}
}
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

public class UserProfileController : BaseController
{
	private readonly UserProfileManagement _userProfileManagement;

	public UserProfileController(UserProfileManagement userProfileManagement, ILogger<UserProfileController> logger)
		: base(logger)
	{
		_userProfileManagement = userProfileManagement;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var auth0UserId = GetAuth0UserId();

		UserProfileDto userProfile = await _userProfileManagement.GetUserProfile(auth0UserId);

		return Ok(userProfile);
	}

	[HttpPut]
	public async Task<IActionResult> Put([FromBody] UserProfileDto userProfileDto)
	{
		var auth0UserId = GetAuth0UserId();

		await _userProfileManagement.UpdateUserProfile(userProfileDto, auth0UserId);

		return NoContent();
	}
}
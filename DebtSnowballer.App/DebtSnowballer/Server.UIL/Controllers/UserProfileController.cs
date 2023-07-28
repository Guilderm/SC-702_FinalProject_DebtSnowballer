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

	[HttpPost]
	public async Task<IActionResult> GetValidateUserProfile([FromBody] UserProfileDto rawUserProfile)
	{
		UserProfileDto validatedUserProfile =
			await _userProfileManagement.GetValidateUserProfile(rawUserProfile, GetAuth0UserId());
		return Ok(validatedUserProfile);
	}
}
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

public class UserProfileController : BaseController
{
	private readonly ILogger<UserProfileController> _logger;
	private readonly UserProfileManagement _userProfileManagement;

	public UserProfileController(UserProfileManagement userProfileManagement, ILogger<UserProfileController> logger)
		: base(logger)
	{
		_userProfileManagement = userProfileManagement;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> GetValidateUserProfile([FromBody] UserProfileDto rawUserProfile)
	{
		_logger.LogInformation(
			"Entered function 'GetValidateUserProfile' in 'UserProfileController' with HTTP POST request. Input: {@rawUserProfile}",
			rawUserProfile);

		UserProfileDto validatedUserProfile =
			await _userProfileManagement.GetValidateUserProfile(rawUserProfile, GetAuth0UserId());

		_logger.LogInformation("Outcome of 'GetValidateUserProfile': {@validatedUserProfile}", validatedUserProfile);

		return Ok(validatedUserProfile);
	}

	[HttpPut("UpdateBaseCurrency/{baseCurrency}")]
	public async Task<IActionResult> UpdateBaseCurrency(string baseCurrency)
	{
		_logger.LogInformation(
			"Entered function 'UpdateBaseCurrency' in 'UserProfileController' with HTTP PUT request. Input: {baseCurrency}",
			baseCurrency);

		bool isUpdated = await _userProfileManagement.UpdateBaseCurrency(baseCurrency, GetAuth0UserId());

		_logger.LogInformation("Outcome of 'UpdateBaseCurrency': {isUpdated}", isUpdated);

		if (isUpdated)
			return Ok();

		return StatusCode(500, "An error occurred while updating the base currency.");
	}
}
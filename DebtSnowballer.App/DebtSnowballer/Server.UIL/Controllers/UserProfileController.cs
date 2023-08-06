using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerServices;

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

	[HttpGet("GetDebtPlanMonthlyPayment")]
	public async Task<IActionResult> GetDebtPlanMonthlyPayment()
	{
		_logger.LogInformation(
			"Entered function 'GetDebtPlanMonthlyPayment' in 'UserProfileController' with HTTP GET request.");

		decimal debtPlanMonthlyPayment = await _userProfileManagement.GetDebtPlanMonthlyPayment(GetAuth0UserId());

		_logger.LogInformation("Outcome of 'GetDebtPlanMonthlyPayment': {debtPlanMonthlyPayment}",
			debtPlanMonthlyPayment);

		return Ok(debtPlanMonthlyPayment);
	}

	[HttpPatch("{strategyTypeId:int}")]
	public async Task<IActionResult> Patch(int strategyTypeId)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Patching User Profile with StrategyTypeID {strategyTypeId} for user {userId}",
			strategyTypeId, auth0UserId);

		var userProfileDto = await _userProfileManagement.UpdateSelectedStrategy(auth0UserId, strategyTypeId);

		if (userProfileDto == null)
			return NotFound($"there was a problem updating user profile not found for Auth0UserId: {auth0UserId}");

		_logger.LogInformation("Patched User Profile with StrategyTypeID {strategyTypeId} for user {userId}",
			strategyTypeId, auth0UserId);

		return Ok(userProfileDto);
	}
}
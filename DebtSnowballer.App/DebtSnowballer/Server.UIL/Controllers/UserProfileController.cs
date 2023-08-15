using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerSideServices;

namespace Server.UIL.Controllers;

public class UserProfileController : BaseController
{
	private readonly ILogger<UserProfileController> _logger;
	private readonly UserPreferenceManagement _userPreferenceManagement;

	public UserProfileController(UserPreferenceManagement userPreferenceManagement,
		ILogger<UserProfileController> logger)
		: base(logger)
	{
		_userPreferenceManagement = userPreferenceManagement;
		_logger = logger;
	}

	[HttpPut("UpdateBaseCurrency/{baseCurrency}")]
	public async Task<IActionResult> UpdateBaseCurrency(string baseCurrency)
	{
		_logger.LogInformation(
			"Entered function 'UpdateBaseCurrency' in 'UserProfileController' with HTTP PUT request. Input: {baseCurrency}",
			baseCurrency);

		bool isUpdated = await _userPreferenceManagement.UpdateBaseCurrency(baseCurrency, GetAuth0UserId());

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

		decimal debtPlanMonthlyPayment = await _userPreferenceManagement.GetDebtPlanMonthlyPayment(GetAuth0UserId());

		_logger.LogInformation("Outcome of 'GetDebtPlanMonthlyPayment': {debtPlanMonthlyPayment}",
			debtPlanMonthlyPayment);

		return Ok(debtPlanMonthlyPayment);
	}

	[HttpPatch("PatchSelectedStrategy/{strategyTypeId:int}")]
	public async Task<IActionResult> PatchSelectedStrategy(int strategyTypeId)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Patching User Profile with StrategyTypeID {strategyTypeId} for user {userId}",
			strategyTypeId, auth0UserId);

		UserPreferenceDto? userProfileDto =
			await _userPreferenceManagement.PatchSelectedStrategy(auth0UserId, strategyTypeId);

		if (userProfileDto == null)
			return NotFound($"there was a problem updating user profile not found for Auth0UserId: {auth0UserId}");

		_logger.LogInformation("Patched User Profile with StrategyTypeID {strategyTypeId} for user {userId}",
			strategyTypeId, auth0UserId);

		return Ok(userProfileDto);
	}
}
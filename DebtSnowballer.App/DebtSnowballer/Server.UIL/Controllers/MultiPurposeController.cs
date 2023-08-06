using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerServices;

namespace Server.UIL.Controllers;

[ApiController]
public class MultiPurposeController : BaseController
{
	private readonly ILogger<MultiPurposeController> _logger;
	private readonly MultiPurposeManagement _multiPurposeManagement;

	public MultiPurposeController(ILogger<MultiPurposeController> logger, MultiPurposeManagement multiPurposeManagement)
		: base(logger)
	{
		_logger = logger;
		_multiPurposeManagement = multiPurposeManagement;
	}

	[HttpGet("GetAllStrategyTypes")]
	public async Task<IActionResult> GetAllStrategyTypes()
	{
		_logger.LogInformation("Getting all strategy types.");

		try
		{
			IList<StrategyTypeDto>? result = await _multiPurposeManagement.GetAllStrategyTypes();
			_logger.LogInformation($"Retrieved {result?.Count} strategy types.");
			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting strategy types.");
			return StatusCode(500, "Internal server error");
		}
	}
}
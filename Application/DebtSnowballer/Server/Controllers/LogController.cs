using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DebtSnowballer.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
	private readonly ILogger<LogController> _logger;

	public LogController(ILogger<LogController> logger)
	{
		_logger = logger;
	}

	[HttpPost]
	public IActionResult Post([FromBody] LogMessage logMessage)
	{
		switch (logMessage.LogLevel)
		{
			case LogLevel.Trace:
				_logger.LogTrace(logMessage.Message);
				break;
			case LogLevel.Debug:
				_logger.LogDebug(logMessage.Message);
				break;
			case LogLevel.Information:
				_logger.LogInformation(logMessage.Message);
				break;
			case LogLevel.Warning:
				_logger.LogWarning(logMessage.Message);
				break;
			case LogLevel.Error:
				_logger.LogError(logMessage.Message);
				break;
			case LogLevel.Critical:
				_logger.LogCritical(logMessage.Message);
				break;
			default:
				_logger.LogInformation(logMessage.Message);
				break;
		}

		return Ok();
	}
}
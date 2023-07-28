using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Server.UIL.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
	protected readonly ILogger _logger;

	protected BaseController(ILogger logger)
	{
		_logger = logger;
	}

	protected string GetAuth0UserId()
	{
		var auth0UserId = User.Claims
			.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
			?.Value;

		if (string.IsNullOrEmpty(auth0UserId))
			_logger.LogError("Auth0 User ID claim is null or empty.");

		return auth0UserId;
	}
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Server.UIL.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
	protected readonly ILogger Logger;

	protected BaseController(ILogger logger)
	{
		Logger = logger;
	}

	protected string GetAuth0UserId()
	{
		string? auth0UserId = User.Claims
			.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
			?.Value;

		if (string.IsNullOrEmpty(auth0UserId))
			Logger.LogError("Auth0 User ID claim is null or empty.");

		return auth0UserId;
	}
}
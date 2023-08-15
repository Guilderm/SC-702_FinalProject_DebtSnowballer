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


}
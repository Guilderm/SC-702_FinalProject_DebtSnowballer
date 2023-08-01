﻿using System.Security.Claims;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IUserProfileService
{
	Task<UserProfileDto> CreateUpdateUserProfile(ClaimsPrincipal user);
	Task UpdateBaseCurrency(string baseCurrency);
	Task UpdatePreferredMonthlyPayment(decimal preferredMonthlyPayment);
}
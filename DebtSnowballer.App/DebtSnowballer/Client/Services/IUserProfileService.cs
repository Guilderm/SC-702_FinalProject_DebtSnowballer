using DebtSnowballer.Shared.DTOs;
using System.Security.Claims;

namespace DebtSnowballer.Client.Services;

public interface IUserProfileService
{
	Task<UserProfileDto> GetUserProfile(ClaimsPrincipal user);
	Task UpdateBaseCurrency(string baseCurrency);
}
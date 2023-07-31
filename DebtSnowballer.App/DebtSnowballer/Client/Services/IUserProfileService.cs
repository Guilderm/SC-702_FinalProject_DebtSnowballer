using System.Security.Claims;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IUserProfileService
{
	Task<UserProfileDto> GetUserProfile(ClaimsPrincipal user);
	Task UpdateBaseCurrency(string baseCurrency);
}
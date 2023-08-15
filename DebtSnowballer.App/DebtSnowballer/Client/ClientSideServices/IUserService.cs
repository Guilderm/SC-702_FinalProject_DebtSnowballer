using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IUserService
{
	Task<UserPreferenceDto> GetUserPreferenceAsync();
	Task<UserPreferenceDto> UpdateUserPreferenceAsync(UserPreferenceDto userPreferenceDto);
	Task<UserProfileDto> GetUserProfileAsync();
}
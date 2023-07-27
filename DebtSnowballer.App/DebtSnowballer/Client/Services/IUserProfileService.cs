using System.Threading.Tasks;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services
{
	public interface IUserProfileService
	{
		Task UpdateBaseCurrency(string baseCurrency);
		Task<UserProfileDto> GetUserProfile();
	}
}
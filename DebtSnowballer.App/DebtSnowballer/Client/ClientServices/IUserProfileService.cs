using System.Security.Claims;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientServices;

public interface IUserProfileService
{
	Task<UserProfileDto> CreateUpdateUserProfile(ClaimsPrincipal user);
	Task UpdateBaseCurrency(string baseCurrency);
	Task UpdateDebtPlanMonthlyPayment(decimal debtPlanMonthlyPayment);
	Task<decimal> GetDebtPlanMonthlyPayment();
	Task<UserProfileDto> UpdateSelectedStrategy(int strategyTypeId);
}
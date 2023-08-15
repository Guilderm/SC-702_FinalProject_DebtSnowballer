using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IUserProfileService
{
	Task UpdateBaseCurrency(string baseCurrency);
	Task UpdateDebtPlanMonthlyPayment(decimal debtPlanMonthlyPayment);
	Task<decimal> GetDebtPlanMonthlyPayment();
	Task<UserProfileDto> UpdateSelectedStrategy(int strategyTypeId);
}
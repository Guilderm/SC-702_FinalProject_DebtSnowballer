using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public interface IFinancialFreedomPlanner
{
	Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDetailDto> debts, List<PlannedSnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment);
}
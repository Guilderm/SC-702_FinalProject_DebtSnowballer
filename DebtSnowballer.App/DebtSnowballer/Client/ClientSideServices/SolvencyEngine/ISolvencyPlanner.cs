using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public interface ISolvencyPlanner
{
	Task<SolvencyPlan> CalculatePaymentPlansAsync(List<LoanDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment);
}
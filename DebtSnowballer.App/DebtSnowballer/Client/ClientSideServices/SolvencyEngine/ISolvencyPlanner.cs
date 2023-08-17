using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public interface ISolvencyPlanner
{
	Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment);
}
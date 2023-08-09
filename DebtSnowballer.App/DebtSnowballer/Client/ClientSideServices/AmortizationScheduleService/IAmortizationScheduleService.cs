using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public interface IAmortizationScheduleService
{
	Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<LoanDetailDto> debts, List<PlannedSnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment);
}
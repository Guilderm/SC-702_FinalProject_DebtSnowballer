using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public interface IAmortizationScheduleService
{
	Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<DebtDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment);
}
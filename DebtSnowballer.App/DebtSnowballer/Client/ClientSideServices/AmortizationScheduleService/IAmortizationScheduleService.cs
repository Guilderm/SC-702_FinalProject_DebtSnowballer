using DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;
using DebtSnowballer.Shared.DTOs;

public interface IAmortizationScheduleService
{
	Task<Dictionary<string, List<PaymentPlanDetail>>> CalculatePaymentPlansAsync(List<DebtDto> debts,
		List<SnowflakeDto> snowflakes, decimal debtPlanMonthlyPayment);
}
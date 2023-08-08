using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleService : IAmortizationScheduleService
{
	public async Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<DebtDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		// Instantiate PaymentPlanCalculator directly
		var paymentPlanCalculator = new PaymentPlanCalculator(new AmortizationScheduleCalculator());

		// Here you can use the snowflakes and debtPlanMonthlyPayment to modify the debts as needed before passing them to the PaymentPlanCalculator

		return await paymentPlanCalculator.CalculatePaymentPlansAsync(debts);
	}
}
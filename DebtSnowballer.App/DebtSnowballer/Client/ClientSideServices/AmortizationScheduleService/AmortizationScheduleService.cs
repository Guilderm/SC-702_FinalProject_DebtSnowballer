using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleService : IAmortizationScheduleService
{
	public async Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<DebtDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		// Determine the maximum amount of time that the schedule will last
		int maxTime = debts.Max(d => d.RemainingTermInMonths);
		SnowflakesScheduleCalculator snowflakesScheduleCalculator = new();
		var snowflakesSchedule = snowflakesScheduleCalculator.CalculateSnowflakes(snowflakes, maxTime);


		decimal totalMonthlyPayments = debts.Sum(d => d.MonthlyPayment);

		if (debtPlanMonthlyPayment < totalMonthlyPayments) debtPlanMonthlyPayment = totalMonthlyPayments;

		debtPlanMonthlyPayment -= totalMonthlyPayments;

		PaymentPlanCalculator paymentPlanCalculator = new(new AmortizationScheduleCalculator());

		return await paymentPlanCalculator.CalculatePaymentPlansAsync(debts);
	}
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleService
{
}

public class AmortizationScheduleService : IAmortizationScheduleService
{
	public async Task<Dictionary<string, List<PaymentPlanDetail>>> CalculatePaymentPlansAsync(List<DebtDto> debts,
		List<SnowflakeDto> snowflakes, decimal debtPlanMonthlyPayment)
	{
		// Initialize the dictionary to hold the payment plans for each strategy
		var paymentPlans = new Dictionary<string, List<PaymentPlanDetail>>();

		// Calculate the payment plan for the Baseline strategy
		var baselinePlanCalculator = new BaselinePlanCalculator();
		var baselinePlan = await baselinePlanCalculator.CalculatePlanAsync(debts, snowflakes, debtPlanMonthlyPayment);
		paymentPlans.Add("Baseline", baselinePlan);

		// Calculate the payment plan for the Snowball strategy
		var snowballPlanCalculator = new SnowballPlanCalculator();
		var snowballPlan = await snowballPlanCalculator.CalculatePlanAsync(debts, snowflakes, debtPlanMonthlyPayment);
		paymentPlans.Add("Snowball", snowballPlan);

		// Calculate the payment plan for the Avalanche strategy
		var avalanchePlanCalculator = new AvalanchePlanCalculator();
		var avalanchePlan = await avalanchePlanCalculator.CalculatePlanAsync(debts, snowflakes, debtPlanMonthlyPayment);
		paymentPlans.Add("Avalanche", avalanchePlan);

		// Calculate the payment plan for the Custom strategy
		var customPlanCalculator = new CustomPlanCalculator();
		var customPlan = await customPlanCalculator.CalculatePlanAsync(debts, snowflakes, debtPlanMonthlyPayment);
		paymentPlans.Add("Custom", customPlan);

		return paymentPlans;
	}
}
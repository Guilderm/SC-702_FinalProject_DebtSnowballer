using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class DebtPayoffPlanCreator
{
	private readonly AmortizationScheduleCreator _scheduleCreator;

	public DebtPayoffPlanCreator(AmortizationScheduleCreator amortizationScheduleCreator)
	{
		_scheduleCreator = amortizationScheduleCreator;
		Console.WriteLine("PaymentPlanCalculator initialized with AmortizationScheduleCalculator");
	}

	public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDetailDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculatePaymentPlansAsync' with debts count: {debts.Count}");

		DebtPayoffPlan debtPayoffPlan = new();

		List<LoanDetailDto> unsortedDebtsForBaseline = debts;
		Console.WriteLine("Calculating Baseline payment plan...");
		debtPayoffPlan.PaymentPlans["Baseline"] =
			await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(unsortedDebtsForBaseline));

		List<LoanDetailDto> sortedDebtsForSnowball = debts.OrderByDescending(d => d.RemainingPrincipal).ToList();
		Console.WriteLine("Calculating Snowball payment plan...");
		debtPayoffPlan.PaymentPlans["Snowball"] =
			await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(sortedDebtsForSnowball));

		List<LoanDetailDto> sortedDebtsForAvalanche = debts.OrderByDescending(d => d.AnnualInterestRate).ToList();
		Console.WriteLine("Calculating Avalanche payment plan...");
		debtPayoffPlan.PaymentPlans["Avalanche"] =
			await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(sortedDebtsForAvalanche));

		List<LoanDetailDto> sortedDebtsForCustom = debts.OrderByDescending(d => d.CardinalOrder).ToList();
		Console.WriteLine("Calculating Custom payment plan...");
		debtPayoffPlan.PaymentPlans["Custom"] =
			await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(sortedDebtsForCustom));

		Console.WriteLine("Successfully calculated all payment plans");
		return debtPayoffPlan;
	}
}
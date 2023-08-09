using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class PaymentPlanCalculator
{
	private readonly AmortizationScheduleCalculator _scheduleCalculator;

	public PaymentPlanCalculator(AmortizationScheduleCalculator amortizationScheduleCalculator)
	{
		_scheduleCalculator = amortizationScheduleCalculator;
		Console.WriteLine("PaymentPlanCalculator initialized with AmortizationScheduleCalculator");
	}

	public async Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<LoanDetailDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculatePaymentPlansAsync' with debts count: {debts.Count}");

		PaymentPlanDetail paymentPlanDetail = new();

		List<LoanDetailDto> unsortedDebtsForBaseline = debts;
		Console.WriteLine("Calculating Baseline payment plan...");
		paymentPlanDetail.PaymentPlans["Baseline"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(unsortedDebtsForBaseline));

		List<LoanDetailDto> sortedDebtsForSnowball = debts.OrderByDescending(d => d.RemainingPrincipal).ToList();
		Console.WriteLine("Calculating Snowball payment plan...");
		paymentPlanDetail.PaymentPlans["Snowball"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForSnowball));

		List<LoanDetailDto> sortedDebtsForAvalanche = debts.OrderByDescending(d => d.AnnualInterestRate).ToList();
		Console.WriteLine("Calculating Avalanche payment plan...");
		paymentPlanDetail.PaymentPlans["Avalanche"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForAvalanche));

		List<LoanDetailDto> sortedDebtsForCustom = debts.OrderByDescending(d => d.CardinalOrder).ToList();
		Console.WriteLine("Calculating Custom payment plan...");
		paymentPlanDetail.PaymentPlans["Custom"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForCustom));

		Console.WriteLine("Successfully calculated all payment plans");
		return paymentPlanDetail;
	}
}
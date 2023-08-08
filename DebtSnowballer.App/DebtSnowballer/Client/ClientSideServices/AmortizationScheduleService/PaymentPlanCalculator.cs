using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class PaymentPlanCalculator
{
	private readonly AmortizationScheduleCalculator _scheduleCalculator;

	public PaymentPlanCalculator(AmortizationScheduleCalculator amortizationScheduleCalculator)
	{
		_scheduleCalculator = amortizationScheduleCalculator;
	}

	public async Task<PaymentPlanDetail> CalculatePaymentPlansAsync(List<DebtDto> debts)
	{
		PaymentPlanDetail paymentPlanDetail = new();

		List<DebtDto> unsortedDebtsForBaseline = debts;
		paymentPlanDetail.PaymentPlans["Baseline"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(unsortedDebtsForBaseline));

		List<DebtDto> sortedDebtsForSnowball = debts.OrderByDescending(d => d.RemainingPrincipal).ToList();
		paymentPlanDetail.PaymentPlans["Snowball"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForSnowball));

		List<DebtDto> sortedDebtsForAvalanche = debts.OrderByDescending(d => d.AnnualInterestRate).ToList();
		paymentPlanDetail.PaymentPlans["Avalanche"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForAvalanche));

		List<DebtDto> sortedDebtsForCustom = debts.OrderByDescending(d => d.CardinalOrder).ToList();
		paymentPlanDetail.PaymentPlans["Custom"] =
			await Task.Run(() => _scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForCustom));

		return paymentPlanDetail;
	}
}
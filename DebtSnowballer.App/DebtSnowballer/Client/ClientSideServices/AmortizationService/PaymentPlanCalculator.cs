using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPlanCalculator
{
	private readonly List<DebtDto> _debts;

	public PaymentPlanCalculator(List<DebtDto> debts)
	{
		_debts = debts;
	}

	public PaymentPlanDetail CalculatePaymentPlan()
	{
		PaymentPlanDetail paymentPlanDetail = new()
		{
			AmortizationSchedules = new List<AmortizationScheduleDetails>()
		};

		AmortizationScheduleCalculator amortizationScheduleCalculator = new(_debts);
		paymentPlanDetail.AmortizationSchedules = amortizationScheduleCalculator.CalculateAmortizationSchedules();

		return paymentPlanDetail;
	}
}
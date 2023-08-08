namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPlanDetail
{
	public PaymentPlanDetail()
	{
		PaymentPlans = new Dictionary<string, List<AmortizationScheduleDetails>>();
	}

	public Dictionary<string, List<AmortizationScheduleDetails>> PaymentPlans { get; set; }
}
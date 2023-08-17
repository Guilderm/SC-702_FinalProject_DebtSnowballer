namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class DebtPayoffPlan
{
	public DebtPayoffPlan()
	{
		PaymentPlans = new Dictionary<string, List<AmortizationSchedule>>();
	}

	public Dictionary<string, List<AmortizationSchedule>> PaymentPlans { get; set; }
}
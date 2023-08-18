namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class SolvencyPlan
{
	public SolvencyPlan()
	{
		PaymentPlans = new();
	}

	public string Auth0UserId { get; set; }
	public Dictionary<string, List<LoanAmortization>> PaymentPlans { get; set; }
}
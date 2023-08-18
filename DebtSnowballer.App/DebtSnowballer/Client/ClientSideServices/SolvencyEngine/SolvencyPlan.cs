namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class SolvencyPlan
{
	public SolvencyPlan()
	{
		PaymentPlans = new Dictionary<string, List<LoanAmortization>>();
	}

	public string Auth0UserId { get; set; }
	public Dictionary<string, List<LoanAmortization>> PaymentPlans { get; set; }
}
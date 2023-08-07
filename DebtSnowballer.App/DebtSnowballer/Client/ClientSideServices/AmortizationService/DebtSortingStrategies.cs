namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class DebtSortingStrategies
{
	public void SortByBalance(List<Debt> debts)
	{
		debts.Sort((a, b) => a.Balance.CompareTo(b.Balance));
	}

	public void SortByInterestRate(List<Debt> debts)
	{
		debts.Sort((a, b) => b.AnnualInterestRate.CompareTo(a.AnnualInterestRate));
	}
}
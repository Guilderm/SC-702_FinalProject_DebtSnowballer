using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class DebtSortingStrategies
{
	public void SortByBalance(List<DebtDto> debts)
	{
		debts.Sort((a, b) => a.RemainingPrincipal.CompareTo(b.RemainingPrincipal));
	}

	public void SortByInterestRate(List<DebtDto> debts)
	{
		debts.Sort((a, b) => b.InterestRate.CompareTo(a.InterestRate));
	}
}
namespace Server.DAL.Models;

public class UserPreference
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; } = null!;
	public string BaseCurrency { get; set; } = null!;
	public decimal DebtPlanMonthlyPayment { get; set; }
	public int SelectedStrategy { get; set; }

	public virtual UserProfile Auth0User { get; set; } = null!;
	public virtual Currency BaseCurrencyNavigation { get; set; } = null!;
	public virtual DebtPayDownMethod SelectedStrategyNavigation { get; set; } = null!;
}
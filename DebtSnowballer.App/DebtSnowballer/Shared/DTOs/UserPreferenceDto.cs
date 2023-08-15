namespace DebtSnowballer.Shared.DTOs;

public class UserPreferenceDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string BaseCurrency { get; set; }
	public decimal DebtPlanMonthlyPayment { get; set; }
	public int SelectedStrategy { get; set; }
	public string BaseCurrencyName { get; set; }
	public string SelectedStrategyName { get; set; }
}
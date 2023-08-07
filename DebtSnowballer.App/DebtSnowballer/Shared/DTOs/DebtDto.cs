namespace DebtSnowballer.Shared.DTOs;

public class DebtDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string NickName { get; set; }
	public decimal RemainingPrincipal { get; set; }
	public decimal BankFees { get; set; }
	public decimal MonthlyPayment { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public int RemainingTermInMonths { get; set; }
	public string CurrencyCode { get; set; }
	public int CardinalOrder { get; set; }
	public DateTime StartDate { get; set; }
}
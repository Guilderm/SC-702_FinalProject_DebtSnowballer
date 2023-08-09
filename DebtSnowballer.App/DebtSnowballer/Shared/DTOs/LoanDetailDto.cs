namespace DebtSnowballer.Shared.DTOs;

public class LoanDetailDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string Name { get; set; }
	public decimal RemainingPrincipal { get; set; }
	public decimal BankFees { get; set; }
	public decimal ContractedMonthlyPayment { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public int RemainingTermInMonths { get; set; }
	public string CurrencyCode { get; set; }
	public int CardinalOrder { get; set; }
	public DateTime StartDate { get; set; }
}
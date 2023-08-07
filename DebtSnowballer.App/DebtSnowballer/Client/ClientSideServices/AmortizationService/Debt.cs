namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class Debt
{
	public decimal Balance { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public int TermInMonths { get; set; }
	public decimal MonthlyBankFee { get; set; }
	public decimal MinimumPayment { get; set; }
}
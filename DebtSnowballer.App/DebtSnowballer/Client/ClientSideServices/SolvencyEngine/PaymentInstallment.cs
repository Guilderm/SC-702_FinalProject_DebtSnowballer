namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class PaymentInstallment
{
	//public LoanDto EndOfMonthLoanState { get; set; }
	public int LoanId { get; set; }
	public string Name { get; set; }

	public decimal RemainingPrincipal { get; set; }
	public DateTime Date { get; set; }
	public int RemainingTermInMonths { get; set; }

	public int Month { get; set; } = 1;
	public decimal InterestPaid { get; set; }
	public decimal BankFeesPaid { get; set; }
	public decimal PrincipalPaid { get; set; }
	public decimal ExtraPayment { get; set; }

	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public decimal TotalPrincipalPaid { get; set; }
	public decimal TotalExtraPayment { get; set; }

	public decimal UnallocatedPayment { get; set; }
}
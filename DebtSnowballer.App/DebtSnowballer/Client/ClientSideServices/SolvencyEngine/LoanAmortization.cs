namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class LoanAmortization
{
	public List<PaymentInstallment> Schedule { get; set; }

	public int DebtId { get; set; }
	public string Name { get; set; }
	public decimal BankFees { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public string CurrencyCode { get; set; }
	public int CardinalOrder { get; set; }

	public decimal ContractedMonthlyPayment { get; set; }

	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public decimal TotalPrincipalPaid { get; set; }
	public decimal TotalExtraPayment { get; set; }
}
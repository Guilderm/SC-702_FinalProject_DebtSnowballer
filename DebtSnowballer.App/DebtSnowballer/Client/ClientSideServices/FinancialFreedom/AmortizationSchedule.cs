namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class AmortizationSchedule
{
	public List<PaymentInstallment> MonthlyDetails { get; set; }

	public int DebtId { get; set; }
	public string Auth0UserId { get; set; }
	public string Name { get; set; }
	public decimal BankFees { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public string CurrencyCode { get; set; }
	public int CardinalOrder { get; set; }

	public decimal ContractedMonthlyPayment { get; set; }

	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public decimal TotalPrincipalPaid { get; set; }
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class PaymentInstallment
{
	public LoanDetailDto EndOfMonthLoanState { get; set; }

	public int PaymentMonth { get; set; }

	public decimal InterestPaid { get; set; }
	public decimal BankFeesPaid { get; set; }
	public decimal PrincipalPaid { get; set; }

	public decimal AccumulatedInterestPaid { get; set; }
	public decimal AccumulatedBankFeesPaid { get; set; }
	public decimal AccumulatedPrincipalPaid { get; set; }
}
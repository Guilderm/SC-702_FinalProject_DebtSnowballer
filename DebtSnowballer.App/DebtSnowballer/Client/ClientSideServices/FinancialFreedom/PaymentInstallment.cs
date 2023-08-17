using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class PaymentInstallment
{
	public LoanDto EndOfMonthLoanState { get; set; }

	public int PaymentMonth { get; set; } = 1;
	public decimal InterestPaid { get; set; }
	public decimal BankFeesPaid { get; set; }
	public decimal PrincipalPaid { get; set; }
	public decimal ExtraPayment { get; set; }

	public decimal AccumulatedInterestPaid { get; set; }
	public decimal AccumulatedBankFeesPaid { get; set; }
	public decimal AccumulatedPrincipalPaid { get; set; }
	public decimal AccumulatedExtraPayment { get; set; }

	public decimal UnallocatedPayment { get; set; }
}
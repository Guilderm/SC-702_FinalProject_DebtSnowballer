using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class MonthlyAmortizationDetail
{
	public DebtDto DebtStateAtMonthEnd { get; set; }

	public int Month { get; set; }
	public DateTime MonthYear { get; set; }

	public decimal StartingBalance { get; set; }
	public decimal InterestPaid { get; set; }
	public decimal AccumulatedInterest { get; set; }
	public decimal BankFeesPaid { get; set; }
	public decimal AccumulatedBankFees { get; set; }
	public decimal PrincipalPaid { get; set; }
	public decimal AmountAmortized { get; set; }
	public decimal ExtraPayment { get; set; }
}
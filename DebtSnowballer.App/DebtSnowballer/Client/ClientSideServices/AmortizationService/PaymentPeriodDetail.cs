using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPeriodDetail
{
	public DebtDto AssociatedDebt { get; set; }

	public int Month { get; set; }
	public decimal InterestPaid { get; set; }
	public decimal AccumulatedInterest { get; set; }
	public decimal AccumulatedBankFees { get; set; }
	public decimal PrincipalPaid { get; set; }
	public decimal AmountAmortized { get; set; }
	public decimal StartingBalance { get; set; }
}
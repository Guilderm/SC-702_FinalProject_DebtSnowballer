namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPeriodDetail
{
	public int Month { get; set; }
	public DateTime PaymentPeriodDate { get; set; }
	public decimal StartingBalance { get; set; }
	public decimal InterestPaid { get; set; }
	public decimal AccumulatedInterest { get; set; }
	public decimal PrincipalPaid { get; set; }
	public decimal BankFee { get; set; }
	public decimal AccumulatedBankFees { get; set; }
	public decimal AmountAmortized { get; set; }
	public decimal EndingBalance { get; set; }
}
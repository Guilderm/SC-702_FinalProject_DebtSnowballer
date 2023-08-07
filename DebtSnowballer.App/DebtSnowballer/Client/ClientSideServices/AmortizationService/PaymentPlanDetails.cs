namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPlanDetails
{
	public DateTime DebtFreeDate { get; set; }
	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public Dictionary<Debt, List<PaymentPeriodDetail>> AmortizationSchedule { get; set; }
}
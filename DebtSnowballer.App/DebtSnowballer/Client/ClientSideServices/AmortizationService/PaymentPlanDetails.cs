using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPlanDetails
{
	public DateTime DebtFreeDate { get; set; }
	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public Dictionary<DebtDto, List<PaymentPeriodDetail>> AmortizationSchedule { get; set; }
}
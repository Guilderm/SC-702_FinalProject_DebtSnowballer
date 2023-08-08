namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationScheduleDetails
{
	public List<MonthlyAmortizationDetail> MonthlyDetails { get; set; }

	public int DebtId { get; set; }
	public decimal ContractedMonthlyPayment { get; set; }
	public decimal AccumulatedInterest { get; set; }
	public decimal AccumulatedBankFees { get; set; }
}
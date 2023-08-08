namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleDetails
{
	public List<MonthlyAmortizationDetail> MonthlyDetails { get; set; }

	public int DebtId { get; set; }
	public string Auth0UserId { get; set; }
	public string NickName { get; set; }
	public decimal BankFees { get; set; }
	public decimal AnnualInterestRate { get; set; }
	public string CurrencyCode { get; set; }
	public int CardinalOrder { get; set; }

	public decimal ContractedMonthlyPayment { get; set; }

	public decimal TotalInterestPaid { get; set; }
	public decimal TotalBankFeesPaid { get; set; }
	public decimal TotalPrincipalPaid { get; set; }
}
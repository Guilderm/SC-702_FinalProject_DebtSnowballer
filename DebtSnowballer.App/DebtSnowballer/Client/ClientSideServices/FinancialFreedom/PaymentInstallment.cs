using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class MonthlyAmortizationDetail
{
	public LoanDetailDto LoanStateAtMonthEnd { get; set; }

	public int Month { get; set; }

	public decimal InterestPaid { get; set; }
	public decimal BankFeesPaid { get; set; }
	public decimal PrincipalPaid { get; set; }

	public decimal AccumulatedInterestPaid { get; set; }
	public decimal AccumulatedBankFeesPaid { get; set; }
	public decimal AccumulatedPrincipalPaid { get; set; }
}
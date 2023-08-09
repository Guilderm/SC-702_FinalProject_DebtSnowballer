using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class MonthlyAmortizationCalculator
{
	private readonly decimal _allocatedExtraPayment;
	private readonly MonthlyAmortizationDetail _finalAmortizationDetail;
	private readonly LoanDetailDto _loanDetailAtMonthEnd;
	private readonly LoanDetailDto _loanDetailAtMonthStart;

	public MonthlyAmortizationCalculator(MonthlyAmortizationDetail debtAtPreviousMonthEnd,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'MonthlyAmortizationCalculator' constructor with extra payment: {allocatedExtraPayment} and debtAtPreviousMonthEnd: {debtAtPreviousMonthEnd} ");

		_loanDetailAtMonthStart = debtAtPreviousMonthEnd?.LoanDetailStateAtMonthEnd
		                          ?? throw new ArgumentNullException(nameof(debtAtPreviousMonthEnd),
			                          "debtAtPreviousMonthEnd and DebtStateAtMonthEnd cannot be null");
		_finalAmortizationDetail = new MonthlyAmortizationDetail();
		_loanDetailAtMonthEnd = new LoanDetailDto();
		_allocatedExtraPayment = allocatedExtraPayment >= 0
			? allocatedExtraPayment
			: throw new ArgumentException("Extra payment cannot be negative", nameof(allocatedExtraPayment));
	}


	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
		Console.WriteLine("Entered function 'CalculateMonthlyDetail'");

		decimal paymentAmount = CalculateMinimumMonthlyPayment(_loanDetailAtMonthStart) + _allocatedExtraPayment;

		_finalAmortizationDetail.InterestPaid =
			_loanDetailAtMonthStart.RemainingPrincipal * (_loanDetailAtMonthStart.AnnualInterestRate / 12);
		_finalAmortizationDetail.AccumulatedInterestPaid += _finalAmortizationDetail.InterestPaid;

		_loanDetailAtMonthEnd.RemainingPrincipal = paymentAmount - _finalAmortizationDetail.InterestPaid -
		                                           _finalAmortizationDetail.BankFeesPaid;

		_finalAmortizationDetail.PrincipalPaid =
			_loanDetailAtMonthStart.RemainingPrincipal - _loanDetailAtMonthEnd.RemainingPrincipal;

		_finalAmortizationDetail.Month++;

		CopyDebtDetails();

		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {_finalAmortizationDetail}");

		return _finalAmortizationDetail;
	}

	private void CopyDebtDetails()
	{
		Console.WriteLine("Entered function 'CopyDebtDetails'");

		_loanDetailAtMonthEnd.ContractedMonthlyPayment = CalculateMinimumMonthlyPayment(_loanDetailAtMonthEnd);
		_loanDetailAtMonthEnd.RemainingTermInMonths = CalculateRemainingTerm(_loanDetailAtMonthEnd);
		_loanDetailAtMonthEnd.StartDate = _loanDetailAtMonthStart.StartDate.AddMonths(1);
	}

	private decimal CalculateMinimumMonthlyPayment(LoanDetailDto loanDetail)
	{
		Console.WriteLine("Entered function 'CalculateMinimumMonthlyPayment'");

		if (loanDetail == null)
			throw new ArgumentNullException(nameof(loanDetail));

		decimal principal = loanDetail.RemainingPrincipal;
		decimal rate = loanDetail.AnnualInterestRate / 12;
		int installments = loanDetail.RemainingTermInMonths;

		Console.WriteLine($"principal is: {principal}");
		Console.WriteLine($"rate is: {rate}");
		Console.WriteLine($"installments is: {installments}");

		// Calculate monthly payment using the standard formula for monthly amortization
		decimal minimumMonthlyPayment = principal * rate * (decimal)Math.Pow(1 + (double)rate, installments) /
		                                ((decimal)Math.Pow(1 + (double)rate, installments) - 1);

		minimumMonthlyPayment += loanDetail.BankFees;

		Console.WriteLine($"Calculated Minimum Monthly Payment: {minimumMonthlyPayment}");

		return minimumMonthlyPayment;
	}

	private int CalculateRemainingTerm(LoanDetailDto loanDetail)
	{
		Console.WriteLine("Entered function 'CalculateRemainingTerm'");

		if (loanDetail == null)
			throw new ArgumentNullException(nameof(loanDetail));

		decimal monthlyPayment = loanDetail.ContractedMonthlyPayment;
		decimal remainingPrincipal = loanDetail.RemainingPrincipal;
		decimal monthlyInterestRate = loanDetail.AnnualInterestRate / 12;

		// Calculate the remaining term using the formula for the number of periods in an ordinary annuity
		int remainingTerm =
			(int)Math.Ceiling(
				Math.Log((double)(monthlyPayment / (monthlyPayment - monthlyInterestRate * remainingPrincipal))) /
				Math.Log(1 + (double)monthlyInterestRate));

		Console.WriteLine($"Calculated Remaining Term: {remainingTerm}");

		return remainingTerm;
	}
}
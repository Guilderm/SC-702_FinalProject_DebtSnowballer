using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class MonthlyAmortizationCalculator
{
	private readonly decimal _allocatedExtraPayment;
	private readonly DebtDto _debtAtMonthEnd;
	private readonly DebtDto _debtAtMonthStart;
	private readonly MonthlyAmortizationDetail _finalAmortizationDetail;

	public MonthlyAmortizationCalculator(MonthlyAmortizationDetail debtAtPreviousMonthEnd,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'MonthlyAmortizationCalculator' constructor with extra payment: {allocatedExtraPayment} and debtAtPreviousMonthEnd: {debtAtPreviousMonthEnd} ");

		_debtAtMonthStart = debtAtPreviousMonthEnd?.DebtStateAtMonthEnd
		                    ?? throw new ArgumentNullException(nameof(debtAtPreviousMonthEnd),
			                    "debtAtPreviousMonthEnd and DebtStateAtMonthEnd cannot be null");
		_finalAmortizationDetail = new MonthlyAmortizationDetail();
		_debtAtMonthEnd = new DebtDto();
		_allocatedExtraPayment = allocatedExtraPayment >= 0
			? allocatedExtraPayment
			: throw new ArgumentException("Extra payment cannot be negative", nameof(allocatedExtraPayment));
	}


	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
		Console.WriteLine("Entered function 'CalculateMonthlyDetail'");

		decimal paymentAmount = CalculateMinimumMonthlyPayment(_debtAtMonthStart) + _allocatedExtraPayment;

		_finalAmortizationDetail.InterestPaid =
			_debtAtMonthStart.RemainingPrincipal * (_debtAtMonthStart.AnnualInterestRate / 12);
		_finalAmortizationDetail.AccumulatedInterestPaid += _finalAmortizationDetail.InterestPaid;

		_debtAtMonthEnd.RemainingPrincipal = paymentAmount - _finalAmortizationDetail.InterestPaid -
		                                     _finalAmortizationDetail.BankFeesPaid;

		_finalAmortizationDetail.PrincipalPaid =
			_debtAtMonthStart.RemainingPrincipal - _debtAtMonthEnd.RemainingPrincipal;

		_finalAmortizationDetail.Month++;

		CopyDebtDetails();

		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {_finalAmortizationDetail}");

		return _finalAmortizationDetail;
	}

	private void CopyDebtDetails()
	{
		Console.WriteLine("Entered function 'CopyDebtDetails'");

		_debtAtMonthEnd.MonthlyPayment = CalculateMinimumMonthlyPayment(_debtAtMonthEnd);
		_debtAtMonthEnd.RemainingTermInMonths = CalculateRemainingTerm(_debtAtMonthEnd);
		_debtAtMonthEnd.StartDate = _debtAtMonthStart.StartDate.AddMonths(1);
	}

	private decimal CalculateMinimumMonthlyPayment(DebtDto debt)
	{
		Console.WriteLine("Entered function 'CalculateMinimumMonthlyPayment'");

		if (debt == null)
			throw new ArgumentNullException(nameof(debt));

		decimal principal = debt.RemainingPrincipal;
		decimal rate = debt.AnnualInterestRate / 12;
		int installments = debt.RemainingTermInMonths;

		Console.WriteLine($"principal is: {principal}");
		Console.WriteLine($"rate is: {rate}");
		Console.WriteLine($"installments is: {installments}");

		// Calculate monthly payment using the standard formula for monthly amortization
		decimal minimumMonthlyPayment = principal * rate * (decimal)Math.Pow(1 + (double)rate, installments) /
		                                ((decimal)Math.Pow(1 + (double)rate, installments) - 1);

		minimumMonthlyPayment += debt.BankFees;

		Console.WriteLine($"Calculated Minimum Monthly Payment: {minimumMonthlyPayment}");

		return minimumMonthlyPayment;
	}

	private int CalculateRemainingTerm(DebtDto debt)
	{
		Console.WriteLine("Entered function 'CalculateRemainingTerm'");

		if (debt == null)
			throw new ArgumentNullException(nameof(debt));

		decimal monthlyPayment = debt.MonthlyPayment;
		decimal remainingPrincipal = debt.RemainingPrincipal;
		decimal monthlyInterestRate = debt.AnnualInterestRate / 12;

		// Calculate the remaining term using the formula for the number of periods in an ordinary annuity
		int remainingTerm =
			(int)Math.Ceiling(
				Math.Log((double)(monthlyPayment / (monthlyPayment - monthlyInterestRate * remainingPrincipal))) /
				Math.Log(1 + (double)monthlyInterestRate));

		Console.WriteLine($"Calculated Remaining Term: {remainingTerm}");

		return remainingTerm;
	}
}
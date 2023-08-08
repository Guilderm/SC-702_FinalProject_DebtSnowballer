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
		_debtAtMonthStart = debtAtPreviousMonthEnd.DebtStateAtMonthEnd;
		_finalAmortizationDetail = new MonthlyAmortizationDetail();
		_debtAtMonthEnd = _finalAmortizationDetail.DebtStateAtMonthEnd;
		_allocatedExtraPayment = allocatedExtraPayment >= 0
			? allocatedExtraPayment
			: throw new ArgumentException("Extra payment cannot be negative", nameof(allocatedExtraPayment));
	}

	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
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

		return _finalAmortizationDetail;
	}

	private void CopyDebtDetails()
	{
		_debtAtMonthEnd.MonthlyPayment = CalculateMinimumMonthlyPayment(_debtAtMonthEnd);
		_debtAtMonthEnd.RemainingTermInMonths = CalculateRemainingTerm(_debtAtMonthEnd);
		_debtAtMonthEnd.StartDate = _debtAtMonthStart.StartDate.AddMonths(1);
	}

	private decimal CalculateMinimumMonthlyPayment(DebtDto debt)
	{
		if (debt == null)
			throw new ArgumentNullException(nameof(debt));

		decimal principal = debt.RemainingPrincipal;
		decimal rate = debt.AnnualInterestRate / 12;
		int installments = debt.RemainingTermInMonths;

		// Calculate monthly payment using the standard formula
		decimal minimumMonthlyPayment = principal * rate * (decimal)Math.Pow(1 + (double)rate, installments) /
		                                ((decimal)Math.Pow(1 + (double)rate, installments) - 1);

		minimumMonthlyPayment += debt.BankFees;

		return minimumMonthlyPayment;
	}

	private int CalculateRemainingTerm(DebtDto debt)
	{
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

		return remainingTerm;
	}
}
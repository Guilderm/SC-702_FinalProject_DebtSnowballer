using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

//This class will take the DebtDto (representing the state of the loan at the start of the month) as input and produce a MonthlyAmortizationDetail as output.
public class MonthlyAmortizationCalculator
{
	private readonly decimal _allocatedExtraPayment;
	private readonly MonthlyAmortizationDetail _debtAtMonthEnd;
	private readonly MonthlyAmortizationDetail _debtAtMonthStart;

	public MonthlyAmortizationCalculator(MonthlyAmortizationDetail debtAtMonthStart, decimal allocatedExtraPayment)
	{
		_debtAtMonthStart = debtAtMonthStart;
		_allocatedExtraPayment = allocatedExtraPayment;
		_debtAtMonthEnd = new MonthlyAmortizationDetail();
	}

	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
		decimal paymentAmount = CalculateMinimumMonthlyPayment(_debtAtMonthStart.DebtStateAtMonthEnd) +
		                        _allocatedExtraPayment;

		_debtAtMonthEnd.InterestPaid =
			_debtAtMonthStart.DebtStateAtMonthEnd.RemainingPrincipal *
			(_debtAtMonthStart.DebtStateAtMonthEnd.AnnualInterestRate / 12);

		_debtAtMonthEnd.DebtStateAtMonthEnd.RemainingPrincipal =
			paymentAmount - _debtAtMonthEnd.InterestPaid - _debtAtMonthEnd.BankFeesPaid;

		_debtAtMonthEnd.DebtStateAtMonthEnd.Id = _debtAtMonthStart.DebtStateAtMonthEnd.Id;
		_debtAtMonthEnd.DebtStateAtMonthEnd.Auth0UserId = _debtAtMonthStart.DebtStateAtMonthEnd.Auth0UserId;
		_debtAtMonthEnd.DebtStateAtMonthEnd.NickName = _debtAtMonthStart.DebtStateAtMonthEnd.NickName;
		_debtAtMonthEnd.DebtStateAtMonthEnd.RemainingPrincipal =
			_debtAtMonthStart.DebtStateAtMonthEnd.RemainingPrincipal - _debtAtMonthEnd.PrincipalPaid;
		_debtAtMonthEnd.DebtStateAtMonthEnd.BankFees = _debtAtMonthStart.DebtStateAtMonthEnd.BankFees;
		_debtAtMonthEnd.DebtStateAtMonthEnd.MonthlyPayment =
			CalculateMinimumMonthlyPayment(_debtAtMonthEnd.DebtStateAtMonthEnd);
		_debtAtMonthEnd.DebtStateAtMonthEnd.AnnualInterestRate =
			_debtAtMonthStart.DebtStateAtMonthEnd.AnnualInterestRate;
		_debtAtMonthEnd.DebtStateAtMonthEnd.RemainingTermInMonths =
			CalculateRemainingTerm(_debtAtMonthEnd.DebtStateAtMonthEnd);
		_debtAtMonthEnd.DebtStateAtMonthEnd.CurrencyCode = _debtAtMonthStart.DebtStateAtMonthEnd.CurrencyCode;
		_debtAtMonthEnd.DebtStateAtMonthEnd.CardinalOrder = _debtAtMonthStart.DebtStateAtMonthEnd.CardinalOrder;
		_debtAtMonthEnd.DebtStateAtMonthEnd.StartDate = _debtAtMonthStart.DebtStateAtMonthEnd.StartDate.AddMonths(1);

		_debtAtMonthEnd.AccumulatedInterest += _debtAtMonthEnd.InterestPaid;
		_debtAtMonthEnd.AccumulatedBankFees += _debtAtMonthEnd.BankFeesPaid;

		return _debtAtMonthEnd;
	}

	private decimal CalculateMinimumMonthlyPayment(DebtDto debt)
	{
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
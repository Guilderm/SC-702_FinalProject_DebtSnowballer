using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

//This class will take the DebtDto (representing the state of the loan at the start of the month) as input and produce a MonthlyAmortizationDetail as output.
public class MonthlyAmortizationCalculator
{
	private readonly decimal _allocatedExtraPayment;
	private readonly DebtDto _debtAtMonthStart;
	private readonly MonthlyAmortizationDetail _monthlyDetail;

	public MonthlyAmortizationCalculator(DebtDto debtAtMonthStart, decimal allocatedExtraPayment)
	{
		_debtAtMonthStart = debtAtMonthStart;
		_allocatedExtraPayment = allocatedExtraPayment;
		_monthlyDetail = new MonthlyAmortizationDetail();
	}

	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
		//TODO the upper class shuld store this in the dto
		_monthlyDetail.MonthYear = DateTime.Now;

		decimal paymentAmount = CalculateMinimumMonthlyPayment(_debtAtMonthStart) + _allocatedExtraPayment;

		_monthlyDetail.InterestPaid =
			_debtAtMonthStart.RemainingPrincipal * (_debtAtMonthStart.AnnualInterestRate / 12);

		if (_debtAtMonthStart.RemainingPrincipal < paymentAmount)
		{
			//TODO: report that not all the allocated amount was used
			//_allocatedExtraPayment = monthlyDetail.StartingBalance;
		}

		_monthlyDetail.DebtStateAtMonthEnd.RemainingPrincipal =
			paymentAmount - _monthlyDetail.InterestPaid - _monthlyDetail.BankFeesPaid;

		_monthlyDetail.DebtStateAtMonthEnd.Id = _debtAtMonthStart.Id;
		_monthlyDetail.DebtStateAtMonthEnd.Auth0UserId = _debtAtMonthStart.Auth0UserId;
		_monthlyDetail.DebtStateAtMonthEnd.NickName = _debtAtMonthStart.NickName;
		_monthlyDetail.DebtStateAtMonthEnd.RemainingPrincipal =
			_debtAtMonthStart.RemainingPrincipal - _monthlyDetail.PrincipalPaid;
		_monthlyDetail.DebtStateAtMonthEnd.BankFees = _debtAtMonthStart.BankFees;
		_monthlyDetail.DebtStateAtMonthEnd.MonthlyPayment =
			CalculateMinimumMonthlyPayment(_monthlyDetail.DebtStateAtMonthEnd);
		_monthlyDetail.DebtStateAtMonthEnd.AnnualInterestRate = _debtAtMonthStart.AnnualInterestRate;
		_monthlyDetail.DebtStateAtMonthEnd.RemainingTermInMonths = _debtAtMonthStart.RemainingTermInMonths - 1;
		_monthlyDetail.DebtStateAtMonthEnd.CurrencyCode = _debtAtMonthStart.CurrencyCode;
		_monthlyDetail.DebtStateAtMonthEnd.CardinalOrder = _debtAtMonthStart.CardinalOrder;
		_monthlyDetail.DebtStateAtMonthEnd.StartDate = _debtAtMonthStart.StartDate.AddMonths(1);

		return _monthlyDetail;
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
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

//This class will take the DebtDto (representing the state of the loan at the start of the month) as input and produce a MonthlyAmortizationDetail as output.
public class MonthlyAmortizationCalculator
{
	private readonly decimal _allocatedExtraPayment;
	private readonly DebtDto _debtAtMonthStart;
	private readonly MonthlyAmortizationDetail _monthlyDetail;

	public MonthlyPaymentCalculator(DebtDto debtAtMonthStart, decimal allocatedExtraPayment)
	{
		_debtAtMonthStart = debtAtMonthStart;
		_allocatedExtraPayment = allocatedExtraPayment;
		_monthlyDetail = new MonthlyAmortizationDetail();
	}

	public MonthlyAmortizationDetail CalculateMonthlyDetail()
	{
		//TODO the upper class shuld store this in the dto
		_monthlyDetail.MonthYear = DateTime.Now; // Assuming current month; adjust as needed
		_monthlyDetail.StartingBalance = _debtAtMonthStart.RemainingPrincipal;

		decimal paymentAmount = CalculateMinimumMonthlyPayment(_debtAtMonthStart) + _allocatedExtraPayment;

		//TODO store interest in months not yearly
		_monthlyDetail.InterestPaid = _monthlyDetail.StartingBalance * (_debtAtMonthStart.AnnualInterestRate / 12);

		if (_debtAtMonthStart.RemainingPrincipal < paymentAmount)
		{
			//TODO: report that not all the allocated amount was used
			//_allocatedExtraPayment = monthlyDetail.StartingBalance;
		}

		_monthlyDetail.DebtStateAtMonthEnd.RemainingPrincipal =
			paymentAmount - _monthlyDetail.InterestPaid - _monthlyDetail.BankFeesPaid;


		// Update DebtStateAtMonthEnd based on calculations
		monthlyDetail.DebtStateAtMonthEnd = new DebtDto
		{
			// ... update properties based on calculations
		};


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
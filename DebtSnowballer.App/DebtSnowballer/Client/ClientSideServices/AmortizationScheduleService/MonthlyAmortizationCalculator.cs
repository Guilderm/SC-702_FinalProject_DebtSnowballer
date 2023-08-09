using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class MonthlyAmortizationCalculator
{
	public MonthlyAmortizationDetail CalculateMonthlyDetail(MonthlyAmortizationDetail loanAtMonthStart,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculateMonthlyDetail' for month {loanAtMonthStart.Month} of date: {loanAtMonthStart.LoanStateAtMonthEnd.StartDate}");

		MonthlyAmortizationDetail loanAtMonthEnd = new()
		{
			LoanStateAtMonthEnd = new LoanDetailDto
			{
				Id = loanAtMonthStart.LoanStateAtMonthEnd.Id,
				Auth0UserId = loanAtMonthStart.LoanStateAtMonthEnd.Auth0UserId,
				Name = loanAtMonthStart.LoanStateAtMonthEnd.Name,
				RemainingPrincipal = loanAtMonthStart.LoanStateAtMonthEnd.RemainingPrincipal,
				BankFees = loanAtMonthStart.LoanStateAtMonthEnd.BankFees,
				ContractedMonthlyPayment = loanAtMonthStart.LoanStateAtMonthEnd.ContractedMonthlyPayment,
				AnnualInterestRate = loanAtMonthStart.LoanStateAtMonthEnd.AnnualInterestRate,
				RemainingTermInMonths = loanAtMonthStart.LoanStateAtMonthEnd.RemainingTermInMonths,
				CurrencyCode = loanAtMonthStart.LoanStateAtMonthEnd.CurrencyCode,
				CardinalOrder = loanAtMonthStart.LoanStateAtMonthEnd.CardinalOrder,
				StartDate = loanAtMonthStart.LoanStateAtMonthEnd.StartDate
			}
		};

		Console.WriteLine(" loanAtMonthStart info is:");
		Console.WriteLine($"  Debt ID: {loanAtMonthStart.LoanStateAtMonthEnd.Id}");
		Console.WriteLine($"  Auth0UserId: {loanAtMonthStart.LoanStateAtMonthEnd.Auth0UserId}");
		Console.WriteLine($"  Name: {loanAtMonthStart.LoanStateAtMonthEnd.Name}");
		Console.WriteLine($"  RemainingPrincipal: {loanAtMonthStart.LoanStateAtMonthEnd.RemainingPrincipal}");
		Console.WriteLine($"  BankFees: {loanAtMonthStart.LoanStateAtMonthEnd.BankFees}");
		Console.WriteLine($"  MonthlyPayment: {loanAtMonthStart.LoanStateAtMonthEnd.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {loanAtMonthStart.LoanStateAtMonthEnd.AnnualInterestRate}");
		Console.WriteLine($"  RemainingTermInMonths: {loanAtMonthStart.LoanStateAtMonthEnd.RemainingTermInMonths}");
		Console.WriteLine($"  CurrencyCode: {loanAtMonthStart.LoanStateAtMonthEnd.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {loanAtMonthStart.LoanStateAtMonthEnd.CardinalOrder}");
		Console.WriteLine($"  StartDate: {loanAtMonthStart.LoanStateAtMonthEnd.StartDate:yyyy-MM-dd}");

		decimal paymentAmount = CalculateMinimumMonthlyPayment(loanAtMonthStart.LoanStateAtMonthEnd) +
		                        allocatedExtraPayment;
		loanAtMonthEnd.InterestPaid =
			loanAtMonthStart.LoanStateAtMonthEnd.RemainingPrincipal *
			(loanAtMonthStart.LoanStateAtMonthEnd.AnnualInterestRate / 12);
		loanAtMonthEnd.BankFeesPaid = loanAtMonthStart.LoanStateAtMonthEnd.BankFees;
		loanAtMonthEnd.PrincipalPaid =
			loanAtMonthStart.LoanStateAtMonthEnd.RemainingPrincipal - (paymentAmount - (loanAtMonthEnd.InterestPaid +
				loanAtMonthEnd.LoanStateAtMonthEnd.BankFees));

		loanAtMonthEnd.LoanStateAtMonthEnd.RemainingPrincipal =
			loanAtMonthStart.LoanStateAtMonthEnd.RemainingPrincipal - loanAtMonthEnd.PrincipalPaid;
		loanAtMonthEnd.LoanStateAtMonthEnd.StartDate = loanAtMonthStart.LoanStateAtMonthEnd.StartDate;
		loanAtMonthEnd.LoanStateAtMonthEnd.RemainingTermInMonths =
			CalculateRemainingTerm(loanAtMonthEnd.LoanStateAtMonthEnd);

		loanAtMonthEnd.AccumulatedInterestPaid = loanAtMonthStart.AccumulatedInterestPaid + loanAtMonthEnd.InterestPaid;
		loanAtMonthEnd.AccumulatedBankFeesPaid = loanAtMonthStart.AccumulatedBankFeesPaid + loanAtMonthEnd.BankFeesPaid;
		loanAtMonthEnd.AccumulatedPrincipalPaid =
			loanAtMonthStart.AccumulatedPrincipalPaid + loanAtMonthEnd.PrincipalPaid;

		loanAtMonthEnd.Month = ++loanAtMonthStart.Month;

		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {loanAtMonthEnd}");
		return loanAtMonthEnd;
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
		decimal monthlyPayment = CalculateMinimumMonthlyPayment(loanDetail);
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
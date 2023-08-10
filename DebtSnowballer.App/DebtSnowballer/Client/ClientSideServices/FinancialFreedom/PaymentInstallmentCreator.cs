using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class PaymentInstallmentCreator
{
	public PaymentInstallment CalculateMonthlyDetail(PaymentInstallment loanAtMonthStart,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculateMonthlyDetail' for month {loanAtMonthStart.Month}" +
			$" of date: {loanAtMonthStart.LoanStateAtMonthEnd.StartDate}");

		PaymentInstallment loanAtMonthEnd = CreateAmortizationDetail(loanAtMonthStart);

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

	private PaymentInstallment CreateAmortizationDetail(PaymentInstallment loanDetail)
	{
		return new PaymentInstallment
		{
			LoanStateAtMonthEnd = new LoanDetailDto
			{
				Id = loanDetail.LoanStateAtMonthEnd.Id,
				Auth0UserId = loanDetail.LoanStateAtMonthEnd.Auth0UserId,
				Name = loanDetail.LoanStateAtMonthEnd.Name,
				RemainingPrincipal = loanDetail.LoanStateAtMonthEnd.RemainingPrincipal,
				BankFees = loanDetail.LoanStateAtMonthEnd.BankFees,
				ContractedMonthlyPayment = loanDetail.LoanStateAtMonthEnd.ContractedMonthlyPayment,
				AnnualInterestRate = loanDetail.LoanStateAtMonthEnd.AnnualInterestRate,
				RemainingTermInMonths = loanDetail.LoanStateAtMonthEnd.RemainingTermInMonths,
				CurrencyCode = loanDetail.LoanStateAtMonthEnd.CurrencyCode,
				CardinalOrder = loanDetail.LoanStateAtMonthEnd.CardinalOrder,
				StartDate = loanDetail.LoanStateAtMonthEnd.StartDate
			}
		};
	}

	private decimal CalculateMinimumMonthlyPayment(LoanDetailDto loanDetail)
	{
		Console.WriteLine("--- Entered function 'CalculateMinimumMonthlyPayment'");

		if (loanDetail == null)
			throw new ArgumentNullException(nameof(loanDetail));

		decimal principal = loanDetail.RemainingPrincipal;
		decimal monthlyInterestRate = loanDetail.AnnualInterestRate / 12;
		int installments = loanDetail.RemainingTermInMonths;

		//Given the precision that can be expected for a program like this there is no need to calculate any further if amount is smaller than 0.001, especially given that very small numbers where generating what are essentially Divide by zero. Presumly because they some times gets rounded down to 0 
		if (principal <= (decimal)0.001 || installments <= 0)
			return 0;

		// Calculate monthly payment using the standard formula for monthly amortization
		decimal rateFactor = (decimal)Math.Pow(1 + (double)monthlyInterestRate, installments);
		decimal minimumMonthlyPayment = principal * monthlyInterestRate * rateFactor / (rateFactor - 1);

		minimumMonthlyPayment += loanDetail.BankFees;

		Console.WriteLine($"Minimum Monthly Payment is calculated to be: {minimumMonthlyPayment}");

		return minimumMonthlyPayment;
	}

	private int CalculateRemainingTerm(LoanDetailDto loanDetail)
	{
		Console.WriteLine("--- Entered function 'CalculateRemainingTerm'");

		if (loanDetail == null)
			throw new ArgumentNullException(nameof(loanDetail));

		if (loanDetail.RemainingPrincipal <= (decimal)0.001) return 0;

		decimal monthlyPayment = CalculateMinimumMonthlyPayment(loanDetail);
		decimal remainingPrincipal = loanDetail.RemainingPrincipal;
		decimal monthlyInterestRate = loanDetail.AnnualInterestRate / 12;

		if (monthlyPayment <= (decimal)0.01) return 0;

		Console.WriteLine($"monthlyPayment is: {monthlyPayment}");
		Console.WriteLine($"remainingPrincipal is:  {remainingPrincipal}");
		Console.WriteLine($"monthlyInterestRate is:  {monthlyInterestRate}");

		// Calculate the remaining term using the formula for the number of periods in an ordinary annuity
		int remainingTerm =
			(int)Math.Ceiling(
				Math.Log((double)(monthlyPayment / (monthlyPayment - monthlyInterestRate * remainingPrincipal))) /
				Math.Log(1 + (double)monthlyInterestRate));

		Console.WriteLine($"Calculated Remaining Term: {remainingTerm}");

		return remainingTerm;
	}
}
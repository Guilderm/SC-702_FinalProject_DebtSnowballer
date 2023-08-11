using DebtSnowballer.Shared.DTOs;
using Microsoft.VisualBasic;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class PaymentInstallmentCreator
{
	public PaymentInstallment CreatePaymentInstallment(PaymentInstallment initialLoanDetail,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CreatePaymentInstallment' for month {initialLoanDetail.PaymentMonth}" +
			$" of date: {initialLoanDetail.EndOfMonthLoanState.StartDate}");

		PaymentInstallment resultingLoanDetail = CreateLoanDetailFromAmortization(initialLoanDetail);

		CalculatePaymentInstallment(initialLoanDetail, allocatedExtraPayment, resultingLoanDetail);

		LogResults(resultingLoanDetail);

		return resultingLoanDetail;
	}

	private void CalculatePaymentInstallment(PaymentInstallment initialLoanDetail, decimal allocatedExtraPayment,
		PaymentInstallment resultingLoanDetail)
	{
		resultingLoanDetail.ExtraPayment = allocatedExtraPayment;
		decimal paymentAmount = initialLoanDetail.EndOfMonthLoanState.ContractedMonthlyPayment +
		                        allocatedExtraPayment;
		resultingLoanDetail.InterestPaid =
			initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal *
			(initialLoanDetail.EndOfMonthLoanState.AnnualInterestRate / 12);
		resultingLoanDetail.BankFeesPaid = initialLoanDetail.EndOfMonthLoanState.BankFees;
		CalculatePrincipalPaid(resultingLoanDetail, paymentAmount);

		CalculateRemainingPrincipal(initialLoanDetail, resultingLoanDetail, paymentAmount);
		resultingLoanDetail.EndOfMonthLoanState.StartDate = initialLoanDetail.EndOfMonthLoanState.StartDate;
		resultingLoanDetail.EndOfMonthLoanState.RemainingTermInMonths =
			CalculateRemainingTerm(resultingLoanDetail.EndOfMonthLoanState);

		resultingLoanDetail.AccumulatedInterestPaid =
			initialLoanDetail.AccumulatedInterestPaid + resultingLoanDetail.InterestPaid;
		resultingLoanDetail.AccumulatedBankFeesPaid =
			initialLoanDetail.AccumulatedBankFeesPaid + resultingLoanDetail.BankFeesPaid;
		resultingLoanDetail.AccumulatedPrincipalPaid =
			initialLoanDetail.AccumulatedPrincipalPaid + resultingLoanDetail.PrincipalPaid;
		resultingLoanDetail.AccumulatedExtraPayment =
			initialLoanDetail.AccumulatedExtraPayment + resultingLoanDetail.ExtraPayment;

		resultingLoanDetail.PaymentMonth = initialLoanDetail.PaymentMonth + 1;
		resultingLoanDetail.EndOfMonthLoanState.StartDate =
			initialLoanDetail.EndOfMonthLoanState.StartDate.AddMonths(1);
	}

	private static void CalculateRemainingPrincipal(PaymentInstallment initialLoanDetail,
		PaymentInstallment resultingLoanDetail, decimal paymentAmount)
	{
		resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal =
			initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal - resultingLoanDetail.PrincipalPaid;
		if (resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal < (decimal)0.001)
		{
			Console.WriteLine("Warning: Payment is larger than principal. Setting PrincipalPaid to zero.");
			resultingLoanDetail.PrincipalPaid = 0;

			resultingLoanDetail.UnallocatedPayment = paymentAmount - (
				resultingLoanDetail.InterestPaid +
				resultingLoanDetail.BankFeesPaid +
				initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal);
			Console.WriteLine($"Unallocated amount is: {resultingLoanDetail.UnallocatedPayment}.");
		}
	}

	private static void CalculatePrincipalPaid(PaymentInstallment resultingLoanDetail, decimal paymentAmount)
	{
		resultingLoanDetail.PrincipalPaid =
			paymentAmount - (resultingLoanDetail.InterestPaid + resultingLoanDetail.BankFeesPaid);
		if (resultingLoanDetail.PrincipalPaid < 0)
		{
			Console.WriteLine(
				"Warning: Interest and Bank Fees exceed the payment amount. Setting PrincipalPaid to zero.");
			resultingLoanDetail.PrincipalPaid = 0;
		}
	}

	private static void LogResults(PaymentInstallment resultingLoanDetail)
	{
		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {resultingLoanDetail}");
		Console.WriteLine($"Interest Paid: {resultingLoanDetail.InterestPaid}");
		Console.WriteLine($"Bank Fees Paid: {resultingLoanDetail.BankFeesPaid}");
		Console.WriteLine($"Principal Paid: {resultingLoanDetail.PrincipalPaid}");
		Console.WriteLine($"Accumulated Interest Paid: {resultingLoanDetail.AccumulatedInterestPaid}");
		Console.WriteLine($"Accumulated Bank Fees Paid: {resultingLoanDetail.AccumulatedBankFeesPaid}");
		Console.WriteLine($"Accumulated Principal Paid: {resultingLoanDetail.AccumulatedPrincipalPaid}");
		Console.WriteLine($"Accumulated Extra Payment: {resultingLoanDetail.AccumulatedExtraPayment}");
		Console.WriteLine($"Unallocated Payment: {resultingLoanDetail.UnallocatedPayment}");
		Console.WriteLine($"Extra Payment: {resultingLoanDetail.ExtraPayment}");
		Console.WriteLine($"Payment Month: {resultingLoanDetail.PaymentMonth}");
		Console.WriteLine(
			$"End of Month Remaining Principal: {resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal}");
		Console.WriteLine($"End of Month Start Date: {resultingLoanDetail.EndOfMonthLoanState.StartDate}");
		Console.WriteLine(
			$"End of Month Remaining Term: {resultingLoanDetail.EndOfMonthLoanState.RemainingTermInMonths}");
	}


	private PaymentInstallment CreateLoanDetailFromAmortization(PaymentInstallment loanDetail)
	{
		return new PaymentInstallment
		{
			EndOfMonthLoanState = new LoanDetailDto
			{
				Id = loanDetail.EndOfMonthLoanState.Id,
				Auth0UserId = loanDetail.EndOfMonthLoanState.Auth0UserId,
				Name = loanDetail.EndOfMonthLoanState.Name,
				RemainingPrincipal = loanDetail.EndOfMonthLoanState.RemainingPrincipal,
				BankFees = loanDetail.EndOfMonthLoanState.BankFees,
				ContractedMonthlyPayment = loanDetail.EndOfMonthLoanState.ContractedMonthlyPayment,
				AnnualInterestRate = loanDetail.EndOfMonthLoanState.AnnualInterestRate,
				RemainingTermInMonths = loanDetail.EndOfMonthLoanState.RemainingTermInMonths,
				CurrencyCode = loanDetail.EndOfMonthLoanState.CurrencyCode,
				CardinalOrder = loanDetail.EndOfMonthLoanState.CardinalOrder,
				StartDate = loanDetail.EndOfMonthLoanState.StartDate
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

		// Given the precision that can be expected for a program like this, there is no need to calculate any further if the amount is smaller than 0.001, especially given that very small numbers were generating what are essentially Divide by zero. Presumably because they sometimes get rounded down to 0 
		if (principal <= 0.001M || installments <= 0)
			return 0;

		Console.WriteLine("--The data to calculate CalculateMinimumMonthlyPayment is:");
		Console.WriteLine($"principal is: {principal}");
		Console.WriteLine($"monthlyInterestRate is:  {monthlyInterestRate}");
		Console.WriteLine($"installments is:  {installments}");

		// Calculate monthly payment using the standard formula for monthly amortization
		decimal rateFactor = (decimal)Math.Pow(1 + (double)monthlyInterestRate, installments);
		decimal minimumMonthlyPayment = principal * monthlyInterestRate * rateFactor / (rateFactor - 1);

		if (minimumMonthlyPayment < (decimal)0.001)
		{
			Console.WriteLine(
				$"Warning: minimum payment is {minimumMonthlyPayment}, wich is bellow 0.001. setting it to to zero.");
			minimumMonthlyPayment = 0;
		}

		// will not Add bank fees, as it will affect how the remaining term is calculated monthly payment

		Console.WriteLine($"Minimum Monthly Payment is calculated to be: {minimumMonthlyPayment}");

		return minimumMonthlyPayment;
	}

	private int CalculateRemainingTerm(LoanDetailDto loanDetail)
	{
		Console.WriteLine("--- Entered function 'CalculateRemainingTerm'");

		if (loanDetail == null)
			throw new ArgumentNullException(nameof(loanDetail));

		if (loanDetail.RemainingPrincipal <= (decimal)0.001)
		{
			Console.WriteLine(
				$"--- Remaining principal is: {loanDetail.RemainingPrincipal},  Wich is bellow 0.001. That is too low to make a meaningful calculation. Will deem the remaining term to be 0");
			return 0;
		}

		decimal monthlyPayment = CalculateMinimumMonthlyPayment(loanDetail);
		decimal remainingPrincipal = loanDetail.RemainingPrincipal;
		decimal monthlyInterestRate = loanDetail.AnnualInterestRate / 12;

		if (monthlyPayment <= (decimal)0.001)
		{
			Console.WriteLine(
				$"--- monthly Payment is: {monthlyPayment},  Wich is bellow 0.001. That is too low to make a meaningful calculation. Will deem the remaining term to be 0");
			return 0;
		}

		if (monthlyPayment < loanDetail.ContractedMonthlyPayment)
		{
			Console.WriteLine(
				$"Monthly payment of {monthlyPayment} is less than the contracted monthly payment of {loanDetail.ContractedMonthlyPayment}. Adjusting monthly payment to match the contracted amount.");
			monthlyPayment = loanDetail.ContractedMonthlyPayment;
		}


		Console.WriteLine("--The data to calculate CalculateRemainingTerm is:");
		Console.WriteLine($"monthlyPayment is: {monthlyPayment}");
		Console.WriteLine($"remainingPrincipal is:  {remainingPrincipal}");
		Console.WriteLine($"monthlyInterestRate is:  {monthlyInterestRate}");

		// Calculate the remaining term using the Financial.NPer function
		double remainingTermDouble = Financial.NPer((double)monthlyInterestRate, -(double)monthlyPayment,
			(double)remainingPrincipal);

		// Round up and convert to integer
		int remainingTerm = (int)Math.Ceiling(remainingTermDouble);

		Console.WriteLine($"Calculated Remaining Term to be: {remainingTerm}");

		return remainingTerm;
	}
}
using Microsoft.VisualBasic;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class PaymentInstallmentCreator
{
	private readonly decimal _contractedMonthlyPayment;
	private readonly decimal _monthlyBankFees;
	private readonly decimal _monthlyInterestRate;

	public PaymentInstallmentCreator(decimal contractedMonthlyPayment, decimal annualInterestRate,
		decimal monthlyBankFees)
	{
		_contractedMonthlyPayment = contractedMonthlyPayment;
		_monthlyBankFees = monthlyBankFees;
		_monthlyInterestRate = annualInterestRate / 12;
	}

	public PaymentInstallment CreatePaymentInstallment(PaymentInstallment previousPayment,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CreatePaymentInstallment' for month {previousPayment.Month}" +
			$" of date: {previousPayment.Date}");

		PaymentInstallment currentPayment = CreateLoanDetailFromAmortization(previousPayment);

		CalculatePaymentInstallment(previousPayment, allocatedExtraPayment, currentPayment);

		return currentPayment;
	}

	private void CalculatePaymentInstallment(PaymentInstallment initialLoanDetail, decimal allocatedExtraPayment,
		PaymentInstallment paymentDetails)
	{
		paymentDetails.ExtraPayment = allocatedExtraPayment;
		decimal paymentAmount = _contractedMonthlyPayment + allocatedExtraPayment;
		paymentDetails.InterestPaid = initialLoanDetail.RemainingPrincipal * _monthlyInterestRate;
		paymentDetails.BankFeesPaid = _monthlyBankFees;
		CalculatePrincipalPaid(paymentDetails, paymentAmount);

		CalculateRemainingPrincipal(initialLoanDetail, paymentDetails, paymentAmount);
		paymentDetails.Date = initialLoanDetail.Date;
		paymentDetails.RemainingTermInMonths = CalculateRemainingTerm(paymentDetails);

		paymentDetails.TotalInterestPaid = initialLoanDetail.TotalInterestPaid + paymentDetails.InterestPaid;
		paymentDetails.TotalBankFeesPaid = initialLoanDetail.TotalBankFeesPaid + paymentDetails.BankFeesPaid;
		paymentDetails.TotalPrincipalPaid = initialLoanDetail.TotalPrincipalPaid + paymentDetails.PrincipalPaid;
		paymentDetails.TotalExtraPayment = initialLoanDetail.TotalExtraPayment + paymentDetails.ExtraPayment;

		paymentDetails.Month = initialLoanDetail.Month + 1;
		paymentDetails.Date = initialLoanDetail.Date.AddMonths(1);
	}

	private static void CalculateRemainingPrincipal(PaymentInstallment initialLoanDetail,
		PaymentInstallment resultingLoanDetail, decimal paymentAmount)
	{
		resultingLoanDetail.RemainingPrincipal =
			initialLoanDetail.RemainingPrincipal - resultingLoanDetail.PrincipalPaid;
		if (resultingLoanDetail.RemainingPrincipal < (decimal)0.001)
		{
			Console.WriteLine("Warning: Payment is larger than principal. Setting PrincipalPaid to zero.");
			resultingLoanDetail.PrincipalPaid = 0;

			resultingLoanDetail.UnallocatedPayment = paymentAmount - (
				resultingLoanDetail.InterestPaid +
				resultingLoanDetail.BankFeesPaid +
				initialLoanDetail.RemainingPrincipal);
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

	private PaymentInstallment CreateLoanDetailFromAmortization(PaymentInstallment payment)
	{
		return new PaymentInstallment
		{
			LoanId = payment.LoanId,
			Name = payment.Name,
			RemainingPrincipal = payment.RemainingPrincipal,
			RemainingTermInMonths = payment.RemainingTermInMonths
		};
	}

	private decimal CalculateMinimumMonthlyPayment(PaymentInstallment payment)
	{
		Console.WriteLine("--- Entered function 'CalculateMinimumMonthlyPayment'");

		if (payment == null)
			throw new ArgumentNullException(nameof(payment));

		decimal principal = payment.RemainingPrincipal;
		decimal monthlyInterestRate = _monthlyInterestRate;
		int installments = payment.RemainingTermInMonths;

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

	private int CalculateRemainingTerm(PaymentInstallment payment)
	{
		Console.WriteLine("--- Entered function 'CalculateRemainingTerm'");

		if (payment == null)
			throw new ArgumentNullException(nameof(payment));

		if (payment.RemainingPrincipal <= (decimal)0.001)
		{
			Console.WriteLine(
				$"--- Remaining principal is: {payment.RemainingPrincipal},  Wich is bellow 0.001. That is too low to make a meaningful calculation. Will deem the remaining term to be 0");
			return 0;
		}

		decimal monthlyPayment = CalculateMinimumMonthlyPayment(payment);
		decimal remainingPrincipal = payment.RemainingPrincipal;
		decimal monthlyInterestRate = _monthlyInterestRate;

		if (monthlyPayment <= (decimal)0.001)
		{
			Console.WriteLine(
				$"--- monthly Payment is: {monthlyPayment},  Wich is bellow 0.001. That is too low to make a meaningful calculation. Will deem the remaining term to be 0");
			return 0;
		}

		if (monthlyPayment < _contractedMonthlyPayment)
		{
			Console.WriteLine(
				$"Monthly payment of {monthlyPayment} is less than the contracted monthly payment of {_contractedMonthlyPayment}. Adjusting monthly payment to match the contracted amount.");
			monthlyPayment = _contractedMonthlyPayment;
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
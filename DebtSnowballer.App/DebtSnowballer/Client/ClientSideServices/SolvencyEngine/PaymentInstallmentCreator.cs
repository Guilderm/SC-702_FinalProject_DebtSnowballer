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
		decimal paymentAmount = _contractedMonthlyPayment + allocatedExtraPayment;
		paymentDetails.InterestPaid = initialLoanDetail.RemainingPrincipal * _monthlyInterestRate;
		paymentDetails.BankFeesPaid = _monthlyBankFees;

		CalculatePrincipalPaid(paymentDetails, paymentAmount);
		CalculateRemainingPrincipal(initialLoanDetail, paymentDetails, paymentAmount);

		paymentDetails.TotalInterestPaid = initialLoanDetail.TotalInterestPaid + paymentDetails.InterestPaid;
		paymentDetails.TotalBankFeesPaid = initialLoanDetail.TotalBankFeesPaid + paymentDetails.BankFeesPaid;
		paymentDetails.TotalPrincipalPaid = initialLoanDetail.TotalPrincipalPaid + paymentDetails.PrincipalPaid;
		paymentDetails.TotalExtraPayment = initialLoanDetail.TotalExtraPayment + allocatedExtraPayment;

		paymentDetails.Month = initialLoanDetail.Month + 1;
		paymentDetails.Date = initialLoanDetail.Date.AddMonths(1);
	}

	private PaymentInstallment CreateLoanDetailFromAmortization(PaymentInstallment payment)
	{
		return new PaymentInstallment
		{
			LoanId = payment.LoanId,
			Name = payment.Name,
			RemainingPrincipal = payment.RemainingPrincipal,
			RemainingTermInMonths = payment.RemainingTermInMonths,
			Date = payment.Date
		};
	}

	private static void CalculatePrincipalPaid(PaymentInstallment resultingLoanDetail, decimal paymentAmount)
	{
		resultingLoanDetail.PrincipalPaid =
			paymentAmount - (resultingLoanDetail.InterestPaid + resultingLoanDetail.BankFeesPaid);
		if (resultingLoanDetail.PrincipalPaid < 0)
		{
			Console.WriteLine(
				"Warning: Interest and Bank Fees exceed paymentAmount. Setting PrincipalPaid to zero.");
			resultingLoanDetail.PrincipalPaid = 0;
		}
	}

	private static void CalculateRemainingPrincipal(PaymentInstallment initialLoanDetail,
		PaymentInstallment paymentDetails, decimal paymentAmount)
	{
		paymentDetails.RemainingPrincipal = initialLoanDetail.RemainingPrincipal - paymentDetails.PrincipalPaid;
		if (paymentDetails.RemainingPrincipal < (decimal)0.01)
		{
			Console.WriteLine("Warning: RemainingPrincipal is less than .01; Will set remainingPrincipal to zero.");
			paymentDetails.RemainingPrincipal = 0;

			paymentDetails.UnallocatedPayment = paymentAmount - (
				paymentDetails.InterestPaid +
				paymentDetails.BankFeesPaid);
			Console.WriteLine($"Unallocated amount is: {paymentDetails.UnallocatedPayment}.");
		}
	}
}
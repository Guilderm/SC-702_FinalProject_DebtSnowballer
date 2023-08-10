using DebtSnowballer.Shared.DTOs;
using Microsoft.VisualBasic;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class PaymentInstallmentCreator
{
	public PaymentInstallment CalculateMonthlyDetail(PaymentInstallment initialLoanDetail,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculateMonthlyDetail' for month {initialLoanDetail.PaymentMonth}" +
			$" of date: {initialLoanDetail.EndOfMonthLoanState.StartDate}");

		PaymentInstallment resultingLoanDetail = CreateLoanDetailFromAmortization(initialLoanDetail);

		Console.WriteLine(" initialLoanDetail info is:");
		Console.WriteLine($"  Debt ID: {initialLoanDetail.EndOfMonthLoanState.Id}");
		Console.WriteLine($"  Auth0UserId: {initialLoanDetail.EndOfMonthLoanState.Auth0UserId}");
		Console.WriteLine($"  Name: {initialLoanDetail.EndOfMonthLoanState.Name}");
		Console.WriteLine($"  RemainingPrincipal: {initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal}");
		Console.WriteLine($"  BankFees: {initialLoanDetail.EndOfMonthLoanState.BankFees}");
		Console.WriteLine(
			$"  ContractedMonthlyPayment: {initialLoanDetail.EndOfMonthLoanState.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {initialLoanDetail.EndOfMonthLoanState.AnnualInterestRate}");
		Console.WriteLine($"  RemainingTermInMonths: {initialLoanDetail.EndOfMonthLoanState.RemainingTermInMonths}");
		Console.WriteLine($"  CurrencyCode: {initialLoanDetail.EndOfMonthLoanState.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {initialLoanDetail.EndOfMonthLoanState.CardinalOrder}");
		Console.WriteLine($"  StartDate: {initialLoanDetail.EndOfMonthLoanState.StartDate:yyyy-MM-dd}");
		Console.WriteLine("-------------------------}");
		Console.WriteLine($"  BankFeesPaid: {initialLoanDetail.BankFeesPaid}");
		Console.WriteLine($"  InterestPaid: {initialLoanDetail.InterestPaid}");
		Console.WriteLine($"  PrincipalPaid: {initialLoanDetail.PrincipalPaid}");
		Console.WriteLine("-------------------------}");
		Console.WriteLine($"  AccumulatedBankFeesPaid: {initialLoanDetail.AccumulatedBankFeesPaid}");
		Console.WriteLine($"  AccumulatedInterestPaid: {initialLoanDetail.AccumulatedInterestPaid}");
		Console.WriteLine($"  AccumulatedPrincipalPaid: {initialLoanDetail.AccumulatedPrincipalPaid}");


		decimal paymentAmount = CalculateMinimumMonthlyPayment(initialLoanDetail.EndOfMonthLoanState) +
		                        allocatedExtraPayment;
		resultingLoanDetail.InterestPaid =
			initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal *
			(initialLoanDetail.EndOfMonthLoanState.AnnualInterestRate / 12);
		resultingLoanDetail.BankFeesPaid = initialLoanDetail.EndOfMonthLoanState.BankFees;
		resultingLoanDetail.PrincipalPaid =
			paymentAmount - (resultingLoanDetail.InterestPaid + resultingLoanDetail.BankFeesPaid);

		resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal =
			initialLoanDetail.EndOfMonthLoanState.RemainingPrincipal - resultingLoanDetail.PrincipalPaid;
		resultingLoanDetail.EndOfMonthLoanState.StartDate = initialLoanDetail.EndOfMonthLoanState.StartDate;
		resultingLoanDetail.EndOfMonthLoanState.RemainingTermInMonths =
			CalculateRemainingTerm(resultingLoanDetail.EndOfMonthLoanState);

		resultingLoanDetail.AccumulatedInterestPaid =
			initialLoanDetail.AccumulatedInterestPaid + resultingLoanDetail.InterestPaid;
		resultingLoanDetail.AccumulatedBankFeesPaid =
			initialLoanDetail.AccumulatedBankFeesPaid + resultingLoanDetail.BankFeesPaid;
		resultingLoanDetail.AccumulatedPrincipalPaid =
			initialLoanDetail.AccumulatedPrincipalPaid + resultingLoanDetail.PrincipalPaid;

		resultingLoanDetail.PaymentMonth = initialLoanDetail.PaymentMonth + 1;

		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {resultingLoanDetail}");

		Console.WriteLine(" resultingLoanDetail info is:");
		Console.WriteLine($"  Debt ID: {resultingLoanDetail.EndOfMonthLoanState.Id}");
		Console.WriteLine($"  Auth0UserId: {resultingLoanDetail.EndOfMonthLoanState.Auth0UserId}");
		Console.WriteLine($"  Name: {resultingLoanDetail.EndOfMonthLoanState.Name}");
		Console.WriteLine($"  RemainingPrincipal: {resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal}");
		Console.WriteLine($"  BankFees: {resultingLoanDetail.EndOfMonthLoanState.BankFees}");
		Console.WriteLine(
			$"  ContractedMonthlyPayment: {resultingLoanDetail.EndOfMonthLoanState.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {resultingLoanDetail.EndOfMonthLoanState.AnnualInterestRate}");
		Console.WriteLine($"  RemainingTermInMonths: {resultingLoanDetail.EndOfMonthLoanState.RemainingTermInMonths}");
		Console.WriteLine($"  CurrencyCode: {resultingLoanDetail.EndOfMonthLoanState.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {resultingLoanDetail.EndOfMonthLoanState.CardinalOrder}");
		Console.WriteLine($"  StartDate: {resultingLoanDetail.EndOfMonthLoanState.StartDate:yyyy-MM-dd}");

		Console.WriteLine($"  BankFeesPaid: {resultingLoanDetail.BankFeesPaid}");
		Console.WriteLine($"  InterestPaid: {resultingLoanDetail.InterestPaid}");
		Console.WriteLine($"  PrincipalPaid: {resultingLoanDetail.PrincipalPaid}");

		Console.WriteLine($"  AccumulatedBankFeesPaid: {resultingLoanDetail.AccumulatedBankFeesPaid}");
		Console.WriteLine($"  AccumulatedInterestPaid: {resultingLoanDetail.AccumulatedInterestPaid}");
		Console.WriteLine($"  AccumulatedPrincipalPaid: {resultingLoanDetail.AccumulatedPrincipalPaid}");

		return resultingLoanDetail;
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

		//Given the precision that can be expected for a program like this there is no need to calculate any further if amount is smaller than 0.001, especially given that very small numbers where generating what are essentially Divide by zero. Presumably because they some times gets rounded down to 0 
		if (principal <= (decimal)0.001 || installments <= 0)
			return 0;


		Console.WriteLine("--The data to calculate CalculateRemainingTerm is:");
		Console.WriteLine($"principal is: {principal}");
		Console.WriteLine($"monthlyInterestRate is:  {monthlyInterestRate}");
		Console.WriteLine($"installments is:  {installments}");

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

		if (monthlyPayment < loanDetail.ContractedMonthlyPayment) monthlyPayment = loanDetail.ContractedMonthlyPayment;

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
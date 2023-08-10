using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class PaymentInstallmentCreator
{
	public PaymentInstallment CalculateMonthlyDetail(PaymentInstallment inicialLoanDetail,
		decimal allocatedExtraPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculateMonthlyDetail' for month {inicialLoanDetail.PaymentMonth}" +
			$" of date: {inicialLoanDetail.EndOfMonthLoanState.StartDate}");

		PaymentInstallment resultingLoanDetail = CreateLoanDetailFromAmortization(inicialLoanDetail);

		Console.WriteLine(" loanAtMonthStart info is:");
		Console.WriteLine($"  Debt ID: {inicialLoanDetail.EndOfMonthLoanState.Id}");
		Console.WriteLine($"  Auth0UserId: {inicialLoanDetail.EndOfMonthLoanState.Auth0UserId}");
		Console.WriteLine($"  Name: {inicialLoanDetail.EndOfMonthLoanState.Name}");
		Console.WriteLine($"  RemainingPrincipal: {inicialLoanDetail.EndOfMonthLoanState.RemainingPrincipal}");
		Console.WriteLine($"  BankFees: {inicialLoanDetail.EndOfMonthLoanState.BankFees}");
		Console.WriteLine($"  MonthlyPayment: {inicialLoanDetail.EndOfMonthLoanState.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {inicialLoanDetail.EndOfMonthLoanState.AnnualInterestRate}");
		Console.WriteLine($"  RemainingTermInMonths: {inicialLoanDetail.EndOfMonthLoanState.RemainingTermInMonths}");
		Console.WriteLine($"  CurrencyCode: {inicialLoanDetail.EndOfMonthLoanState.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {inicialLoanDetail.EndOfMonthLoanState.CardinalOrder}");
		Console.WriteLine($"  StartDate: {inicialLoanDetail.EndOfMonthLoanState.StartDate:yyyy-MM-dd}");

		decimal paymentAmount = CalculateMinimumMonthlyPayment(inicialLoanDetail.EndOfMonthLoanState) +
		                        allocatedExtraPayment;
		resultingLoanDetail.InterestPaid =
			inicialLoanDetail.EndOfMonthLoanState.RemainingPrincipal *
			(inicialLoanDetail.EndOfMonthLoanState.AnnualInterestRate / 12);
		resultingLoanDetail.BankFeesPaid = inicialLoanDetail.EndOfMonthLoanState.BankFees;
		resultingLoanDetail.PrincipalPaid =
			inicialLoanDetail.EndOfMonthLoanState.RemainingPrincipal - (paymentAmount -
			                                                            (resultingLoanDetail.InterestPaid +
			                                                             resultingLoanDetail.EndOfMonthLoanState
				                                                             .BankFees));

		resultingLoanDetail.EndOfMonthLoanState.RemainingPrincipal =
			inicialLoanDetail.EndOfMonthLoanState.RemainingPrincipal - resultingLoanDetail.PrincipalPaid;
		resultingLoanDetail.EndOfMonthLoanState.StartDate = inicialLoanDetail.EndOfMonthLoanState.StartDate;
		resultingLoanDetail.EndOfMonthLoanState.RemainingTermInMonths =
			CalculateRemainingTerm(resultingLoanDetail.EndOfMonthLoanState);

		resultingLoanDetail.AccumulatedInterestPaid =
			inicialLoanDetail.AccumulatedInterestPaid + resultingLoanDetail.InterestPaid;
		resultingLoanDetail.AccumulatedBankFeesPaid =
			inicialLoanDetail.AccumulatedBankFeesPaid + resultingLoanDetail.BankFeesPaid;
		resultingLoanDetail.AccumulatedPrincipalPaid =
			inicialLoanDetail.AccumulatedPrincipalPaid + resultingLoanDetail.PrincipalPaid;

		resultingLoanDetail.PaymentMonth = ++inicialLoanDetail.PaymentMonth;

		Console.WriteLine($"Calculated MonthlyAmortizationDetail: {resultingLoanDetail}");
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
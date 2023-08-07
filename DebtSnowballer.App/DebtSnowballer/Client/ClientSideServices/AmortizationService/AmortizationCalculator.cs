using System.Text.Json;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationCalculator
{
	private readonly ILogger<AmortizationCalculator> _logger;

	public AmortizationCalculator(ILogger<AmortizationCalculator> logger)
	{
		_logger = logger;
	}

	public List<PaymentPeriodDetail> CalculateAmortizationSchedule(decimal loanAmount, decimal annualInterestRate,
		int termInMonths, decimal monthlyBankFee, DateTime startDate)
	{
		_logger.LogInformation(
			"Calculating amortization schedule for loan amount: {LoanAmount}, annual interest rate: {AnnualInterestRate}, term in months: {TermInMonths}, monthly bank fee: {MonthlyBankFee}, start date: {StartDate}",
			loanAmount, annualInterestRate, termInMonths, monthlyBankFee, startDate);

		List<PaymentPeriodDetail> amortizationSchedule = new List<PaymentPeriodDetail>();
		decimal monthlyInterestRate = annualInterestRate / 12 / 100;
		decimal monthlyPayment = loanAmount * monthlyInterestRate /
		                         (1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -termInMonths));

		decimal accumulatedInterest = 0;
		decimal accumulatedBankFees = 0;

		for (int month = 1; month <= termInMonths; month++)
		{
			decimal interestPaid = loanAmount * monthlyInterestRate;
			decimal principalPaid = monthlyPayment - interestPaid;
			decimal endingBalance = loanAmount - principalPaid;
			decimal amountAmortized = principalPaid + interestPaid + monthlyBankFee;

			accumulatedBankFees += monthlyBankFee;
			accumulatedInterest += interestPaid;

			amortizationSchedule.Add(new PaymentPeriodDetail
			{
				Month = month,
				PaymentPeriodDate = startDate.AddMonths(month - 1),
				StartingBalance = loanAmount,
				InterestPaid = interestPaid,
				AccumulatedInterest = accumulatedInterest,
				PrincipalPaid = principalPaid,
				BankFee = monthlyBankFee,
				AccumulatedBankFees = accumulatedBankFees,
				AmountAmortized = amountAmortized,
				EndingBalance = endingBalance
			});

			loanAmount = endingBalance;
		}

		_logger.LogInformation(
			"Amortization schedule calculated successfully for {TermInMonths} months. Total payment periods: {TotalPaymentPeriods}",
			termInMonths, amortizationSchedule.Count);

		JsonSerializerOptions options = new() { WriteIndented = true };
		_logger.LogInformation("Amortization schedule: {AmortizationSchedule}",
			JsonSerializer.Serialize(amortizationSchedule, options));

		return amortizationSchedule;
	}
}
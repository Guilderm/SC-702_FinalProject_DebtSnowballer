using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationCalculator
{
	private readonly ILogger<AmortizationCalculator> _logger;

	public AmortizationCalculator(ILogger<AmortizationCalculator> logger)
	{
		_logger = logger;
	}

	public List<PaymentPeriodDetail> CalculateAmortizationSchedule(DebtDto debt, DateTime startDate,
		decimal extraPayment)
	{
		_logger.LogInformation(
			"Calculating amortization schedule for loan amount: {LoanAmount}, annual interest rate: {AnnualInterestRate}, term in months: {TermInMonths}, monthly bank fee: {MonthlyBankFee}, start date: {StartDate}",
			debt.RemainingPrincipal, debt.InterestRate, debt.RemainingTermInMonths, debt.BankFees, startDate);

		List<PaymentPeriodDetail> amortizationSchedule = new List<PaymentPeriodDetail>();
		decimal monthlyInterestRate = debt.InterestRate / 12 / 100;
		decimal monthlyPayment = debt.RemainingPrincipal * monthlyInterestRate /
			(1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -debt.RemainingTermInMonths)) + extraPayment;

		decimal accumulatedInterest = 0;
		decimal accumulatedBankFees = 0;

		for (int month = 1; month <= debt.RemainingTermInMonths; month++)
		{
			decimal interestPaid = debt.RemainingPrincipal * monthlyInterestRate;
			decimal principalPaid = monthlyPayment - interestPaid;
			decimal endingBalance = debt.RemainingPrincipal - principalPaid;
			decimal amountAmortized = principalPaid + interestPaid + debt.BankFees;

			accumulatedBankFees += debt.BankFees;
			accumulatedInterest += interestPaid;

			amortizationSchedule.Add(new PaymentPeriodDetail
			{
				Month = month,
				PaymentPeriodDate = startDate.AddMonths(month - 1),
				StartingBalance = debt.RemainingPrincipal,
				InterestPaid = interestPaid,
				AccumulatedInterest = accumulatedInterest,
				PrincipalPaid = principalPaid,
				BankFee = debt.BankFees,
				AccumulatedBankFees = accumulatedBankFees,
				AmountAmortized = amountAmortized,
				EndingBalance = endingBalance
			});

			debt.RemainingPrincipal = endingBalance;
		}

		_logger.LogInformation(
			"Amortization schedule calculated successfully for {TermInMonths} months. Total payment periods: {TotalPaymentPeriods}",
			debt.RemainingTermInMonths, amortizationSchedule.Count);

		JsonSerializerOptions options = new() { WriteIndented = true };
		_logger.LogInformation("Amortization schedule: {AmortizationSchedule}",
			JsonSerializer.Serialize(amortizationSchedule, options));

		return amortizationSchedule;
	}
}
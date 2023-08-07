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

	public List<PaymentPeriodDetail> CalculateAmortizationSchedule(DebtDto debt, decimal extraPayment)
	{
		_logger.LogInformation(
			"Calculating amortization schedule for loan amount: {LoanAmount}, annual interest rate: {AnnualInterestRate}, term in months: {TermInMonths}, monthly bank fee: {MonthlyBankFee}, start date: {StartDate}",
			debt.RemainingPrincipal, debt.AnnualInterestRate, debt.RemainingTermInMonths, debt.BankFees,
			debt.StartDate);

		List<PaymentPeriodDetail> amortizationSchedule = new List<PaymentPeriodDetail>();
		decimal monthlyInterestRate = debt.AnnualInterestRate / 12 / 100;

		decimal accumulatedInterest = 0;
		decimal accumulatedBankFees = 0;

		for (int month = 1; month <= debt.RemainingTermInMonths; month++)
		{
			decimal monthlyPayment = debt.RemainingPrincipal * monthlyInterestRate /
				(1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -debt.RemainingTermInMonths)) + extraPayment;

			decimal interestPaid = debt.RemainingPrincipal * monthlyInterestRate;
			decimal principalPaid = monthlyPayment - interestPaid - debt.BankFees;
			decimal endingBalance = debt.RemainingPrincipal - principalPaid;
			decimal amountAmortized = principalPaid + interestPaid + debt.BankFees;

			accumulatedBankFees += debt.BankFees;
			accumulatedInterest += interestPaid;

			amortizationSchedule.Add(new PaymentPeriodDetail
			{
				AssociatedDebt = new DebtDto
				{
					Id = debt.Id,
					Auth0UserId = debt.Auth0UserId,
					NickName = debt.NickName,
					RemainingPrincipal = endingBalance,
					BankFees = debt.BankFees,
					MonthlyPayment = monthlyPayment,
					AnnualInterestRate = debt.AnnualInterestRate,
					RemainingTermInMonths = debt.RemainingTermInMonths - month,
					CurrencyCode = debt.CurrencyCode,
					CardinalOrder = debt.CardinalOrder,
					StartDate = debt.StartDate.AddMonths(month)
				},
				Month = month,
				InterestPaid = interestPaid,
				AccumulatedInterest = accumulatedInterest,
				PrincipalPaid = principalPaid,
				AmountAmortized = amountAmortized,
				EndingBalance = endingBalance
			});

			debt.RemainingPrincipal = endingBalance;
			debt.RemainingTermInMonths--;
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
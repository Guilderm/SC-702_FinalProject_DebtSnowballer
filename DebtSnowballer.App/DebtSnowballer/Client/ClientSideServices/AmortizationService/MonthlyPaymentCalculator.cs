using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class MonthlyPaymentCalculator
{
	private readonly ILogger<MonthlyPaymentCalculator> _logger;

	public MonthlyPaymentCalculator(ILogger<MonthlyPaymentCalculator> logger)
	{
		_logger = logger;
	}

	public List<MonthlyPayment> CalculateAmortizationSchedule(DebtDto debt, decimal extraPayment)
	{
		_logger.LogInformation(
			"Calculating amortization schedule for loan amount: {LoanAmount}, annual interest rate: {AnnualInterestRate}, term in months: {TermInMonths}, monthly bank fee: {MonthlyBankFee}, start date: {StartDate}",
			debt.RemainingPrincipal, debt.AnnualInterestRate, debt.RemainingTermInMonths, debt.BankFees,
			debt.StartDate);

		List<MonthlyPayment> amortizationSchedule = new List<MonthlyPayment>();
		decimal monthlyInterestRate = CalculateMonthlyInterestRate(debt.AnnualInterestRate);

		decimal accumulatedInterest = 0;
		decimal accumulatedBankFees = 0;

		for (int month = 1; month <= debt.RemainingTermInMonths; month++)
		{
			decimal calculatedMonthlyPayment = CalculateMonthlyPayment(debt, monthlyInterestRate, extraPayment);
			decimal interestPaid = CalculateInterestPaid(debt, monthlyInterestRate);
			decimal principalPaid = CalculatePrincipalPaid(calculatedMonthlyPayment, interestPaid, debt.BankFees);

			accumulatedInterest += interestPaid;
			accumulatedBankFees += debt.BankFees;

			MonthlyPayment paymentDetail = CreatePaymentDetail(debt, month, interestPaid, accumulatedInterest,
				accumulatedBankFees, principalPaid, calculatedMonthlyPayment);
			amortizationSchedule.Add(paymentDetail);

			UpdateDebt(debt, principalPaid);
		}

		LogAmortizationSchedule(amortizationSchedule);

		return amortizationSchedule;
	}

	private decimal CalculateMonthlyInterestRate(decimal annualInterestRate)
	{
		return annualInterestRate / 12 / 100;
	}

	private decimal CalculateMonthlyPayment(DebtDto debt, decimal monthlyInterestRate, decimal extraPayment)
	{
		return debt.RemainingPrincipal * monthlyInterestRate /
			(1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -debt.RemainingTermInMonths)) + extraPayment;
	}

	private decimal CalculateInterestPaid(DebtDto debt, decimal monthlyInterestRate)
	{
		return debt.RemainingPrincipal * monthlyInterestRate;
	}

	private decimal CalculatePrincipalPaid(decimal calculatedMonthlyPayment, decimal interestPaid, decimal bankFees)
	{
		return calculatedMonthlyPayment - interestPaid - bankFees;
	}

	private MonthlyPayment CreatePaymentDetail(DebtDto debt, int month, decimal interestPaid,
		decimal accumulatedInterest, decimal accumulatedBankFees, decimal principalPaid,
		decimal calculatedMonthlyPayment)
	{
		return new MonthlyPayment
		{
			AssociatedDebtState = new DebtDto
			{
				Id = debt.Id,
				Auth0UserId = debt.Auth0UserId,
				NickName = debt.NickName,
				RemainingPrincipal = debt.RemainingPrincipal - principalPaid,
				BankFees = debt.BankFees,
				MonthlyPayment = calculatedMonthlyPayment,
				AnnualInterestRate = debt.AnnualInterestRate,
				RemainingTermInMonths = debt.RemainingTermInMonths - month,
				CurrencyCode = debt.CurrencyCode,
				CardinalOrder = debt.CardinalOrder,
				StartDate = debt.StartDate.AddMonths(month)
			},
			Month = month,
			InterestPaid = interestPaid,
			AccumulatedInterest = accumulatedInterest,
			AccumulatedBankFees = accumulatedBankFees,
			PrincipalPaid = principalPaid,
			StartingBalance = debt.RemainingPrincipal
		};
	}

	private void UpdateDebt(DebtDto debt, decimal principalPaid)
	{
		debt.RemainingPrincipal -= principalPaid;
		debt.RemainingTermInMonths--;
	}

	private void LogAmortizationSchedule(List<MonthlyPayment> amortizationSchedule)
	{
		_logger.LogInformation(
			"Amortization schedule calculated successfully for {TermInMonths} months. Total payment periods: {TotalPaymentPeriods}",
			amortizationSchedule.Last().AssociatedDebtState.RemainingTermInMonths, amortizationSchedule.Count);

		JsonSerializerOptions options = new() { WriteIndented = true };
		_logger.LogInformation("Amortization schedule: {AmortizationSchedule}",
			JsonSerializer.Serialize(amortizationSchedule, options));
	}
}
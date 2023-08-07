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

	public List<PaymentPeriodDetail> CalculateAmortizationSchedule(DebtDto debt, decimal extraPayment)
	{
		_logger.LogInformation(
			"Calculating amortization schedule for loan amount: {LoanAmount}, annual interest rate: {AnnualInterestRate}, term in months: {TermInMonths}, monthly bank fee: {MonthlyBankFee}, start date: {StartDate}",
			debt.RemainingPrincipal, debt.AnnualInterestRate, debt.RemainingTermInMonths, debt.BankFees,
			debt.StartDate);

		List<PaymentPeriodDetail> amortizationSchedule = new List<PaymentPeriodDetail>();
		decimal monthlyInterestRate = CalculateMonthlyInterestRate(debt.AnnualInterestRate);

		decimal accumulatedInterest = 0;
		decimal accumulatedBankFees = 0;

		for (int month = 1; month <= debt.RemainingTermInMonths; month++)
		{
			decimal monthlyPayment = CalculateMonthlyPayment(debt, monthlyInterestRate, extraPayment);
			decimal interestPaid = CalculateInterestPaid(debt, monthlyInterestRate);
			decimal principalPaid = CalculatePrincipalPaid(monthlyPayment, interestPaid, debt.BankFees);

			accumulatedInterest += interestPaid;
			accumulatedBankFees += debt.BankFees;

			PaymentPeriodDetail paymentDetail = CreatePaymentDetail(debt, month, interestPaid, accumulatedInterest,
				accumulatedBankFees, principalPaid, monthlyPayment);
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

	private decimal CalculatePrincipalPaid(decimal monthlyPayment, decimal interestPaid, decimal bankFees)
	{
		return monthlyPayment - interestPaid - bankFees;
	}

	private PaymentPeriodDetail CreatePaymentDetail(DebtDto debt, int month, decimal interestPaid,
		decimal accumulatedInterest, decimal accumulatedBankFees, decimal principalPaid, decimal monthlyPayment)
	{
		return new PaymentPeriodDetail
		{
			AssociatedDebtState = new DebtDto
			{
				// ... (same as before)
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

	private void LogAmortizationSchedule(List<PaymentPeriodDetail> amortizationSchedule)
	{
		_logger.LogInformation(
			"Amortization schedule calculated successfully for {TermInMonths} months. Total payment periods: {TotalPaymentPeriods}",
			amortizationSchedule.Last().AssociatedDebtState.RemainingTermInMonths, amortizationSchedule.Count);

		JsonSerializerOptions options = new() { WriteIndented = true };
		_logger.LogInformation("Amortization schedule: {AmortizationSchedule}",
			JsonSerializer.Serialize(amortizationSchedule, options));
	}
}
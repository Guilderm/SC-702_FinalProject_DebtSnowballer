using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class FinancialFreedomPlanner : IFinancialFreedomPlanner
{
	public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDetailDto> debts,
		List<PlannedSnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculatePaymentPlansAsync' with debts: {JsonSerializer.Serialize(debts)}, snowflakes: {JsonSerializer.Serialize(snowflakes)}, debtPlanMonthlyPayment: {debtPlanMonthlyPayment}");

		var debtsSummary = debts.Select(d =>
			$"Id: {d.Id}, " +
			$"Name: {d.Name}, " +
			$"Principal: ${d.RemainingPrincipal:0.00}, " +
			$"Bank Fees: ${d.BankFees:0.00}, " +
			$"Monthly Payment: ${d.ContractedMonthlyPayment:0.00}, " +
			$"Interest Rate: {d.AnnualInterestRate:P2}, " +
			$"Term: {d.RemainingTermInMonths} months, " +
			$"Currency: {d.CurrencyCode}, " +
			$"Order: {d.CardinalOrder}, " +
			$"Start Date: {d.StartDate:yyyy-MM-dd}"
		).ToList();

		Console.WriteLine("Entered function 'CalculatePaymentPlansAsync'");
		Console.WriteLine("Debts:");
		foreach (var debtSummary in debtsSummary) Console.WriteLine($"- {debtSummary}");

		// Determine the maximum amount of time that the schedule will last based on the remaining term in months across all debts
		int maxTime = debts.Max(d => d.RemainingTermInMonths);
		SnowflakesScheduleCreator snowflakesScheduleCreator = new();
		var snowflakesSchedule = snowflakesScheduleCreator.CalculateSnowflakes(snowflakes, maxTime);

		CalcualteExtraPayment(debts, debtPlanMonthlyPayment);

		DebtPayoffPlanCreator debtPayoffPlanCreator = new(new AmortizationScheduleCreator());
		var result = await debtPayoffPlanCreator.CalculatePaymentPlansAsync(debts);

		Console.WriteLine($"Successfully calculated payment plans: {JsonSerializer.Serialize(result)}");

		return result;
	}

	private static void CalcualteExtraPayment(List<LoanDetailDto> debts, decimal debtPlanMonthlyPayment)
	{
		decimal totalMonthlyPayments = debts.Sum(d => d.ContractedMonthlyPayment);

		if (debtPlanMonthlyPayment < totalMonthlyPayments)
		{
			Console.WriteLine(
				$"debtPlanMonthlyPayment is lower than totalMonthlyPayments. So it will be adjusted to match totalMonthlyPayments of: {totalMonthlyPayments}");
			debtPlanMonthlyPayment = totalMonthlyPayments;
		}

		debtPlanMonthlyPayment -= totalMonthlyPayments;
		Console.WriteLine($"Monthly amortization amount is calculated to be: {debtPlanMonthlyPayment}");
	}
}
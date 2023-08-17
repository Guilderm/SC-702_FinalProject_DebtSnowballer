using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class FinancialFreedomPlanner : IFinancialFreedomPlanner
{
	public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(
		List<LoanDto> debts,
		List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		if (debts == null || debts.Count < 1)
		{
			Console.WriteLine(
				"Debts parameter is null or has a count of zero. Exiting function 'CalculatePaymentPlansAsync'.");
			throw new ArgumentNullException(nameof(debts), "Debts cannot be null, or has a count of zero.");
		}

		if (snowflakes == null)
		{
			Console.WriteLine("Snowflakes parameter is null. Exiting function 'CalculatePaymentPlansAsync'.");
			throw new ArgumentNullException(nameof(snowflakes), "Snowflakes cannot be null.");
		}

		if (debtPlanMonthlyPayment < (decimal)0.001)
		{
			Console.WriteLine(
				$"Warning debtPlanMonthlyPayment parameter is {debtPlanMonthlyPayment}. will set it to 0.");
			debtPlanMonthlyPayment = 0;
		}

		Console.WriteLine(
			$"Entered function 'CalculatePaymentPlansAsync' with debts: {JsonSerializer.Serialize(debts)}," +
			$" snowflakes: {JsonSerializer.Serialize(snowflakes)}, debtPlanMonthlyPayment: {debtPlanMonthlyPayment}");


		// Determine the maximum amount of time that the schedule will last based on the remaining term in months across all debts
		int maxTime = debts.Max(d => d.RemainingTermInMonths);
		SnowflakesScheduleCreator snowflakesScheduleCreator = new();
		var snowflakesSchedule = snowflakesScheduleCreator.CalculateSnowflakes(snowflakes, maxTime);

		CalculateExtraPayment(debts, debtPlanMonthlyPayment);


		DebtPayoffPlanCreator debtPayoffPlanCreator = new(new AmortizationScheduleCreator());
		DebtPayoffPlan result = await debtPayoffPlanCreator.CalculatePaymentPlansAsync(debts);

		Console.WriteLine($"Successfully calculated payment plans: {JsonSerializer.Serialize(result)}");

		return result;
	}

	private static void CalculateExtraPayment(List<LoanDto> debts, decimal debtPlanMonthlyPayment)
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
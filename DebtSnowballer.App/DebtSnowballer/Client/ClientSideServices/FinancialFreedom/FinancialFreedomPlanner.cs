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

		// Determine the maximum amount of time that the schedule will last based on the remaining term in months across all debts
		int maxTime = debts.Max(d => d.RemainingTermInMonths);
		SnowflakesScheduleCreator snowflakesScheduleCreator = new();
		var snowflakesSchedule = snowflakesScheduleCreator.CalculateSnowflakes(snowflakes, maxTime);

		decimal totalMonthlyPayments = debts.Sum(d => d.ContractedMonthlyPayment);

		if (debtPlanMonthlyPayment < totalMonthlyPayments)
		{
			Console.WriteLine(
				$"debtPlanMonthlyPayment is lower than totalMonthlyPayments. So it will be adjusted to match totalMonthlyPayments of: {totalMonthlyPayments}");
			debtPlanMonthlyPayment = totalMonthlyPayments;
		}

		debtPlanMonthlyPayment -= totalMonthlyPayments;
		Console.WriteLine($"Monthly amortization amount is calculated to be: {debtPlanMonthlyPayment}");

		DebtPayoffPlanCreator debtPayoffPlanCreator = new(new AmortizationScheduleCreator());
		var result = await debtPayoffPlanCreator.CalculatePaymentPlansAsync(debts);

		Console.WriteLine($"Successfully calculated payment plans: {JsonSerializer.Serialize(result)}");

		return result;
	}
}
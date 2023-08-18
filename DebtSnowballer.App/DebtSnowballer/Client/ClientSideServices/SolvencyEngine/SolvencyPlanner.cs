using System.Text.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class SolvencyPlanner : ISolvencyPlanner
{
	private readonly DebtPayoffPlanCreator _debtPayoffPlanCreator;
	private readonly SnowflakesScheduleCreator _snowflakesScheduleCreator;

	public SolvencyPlanner(DebtPayoffPlanCreator debtPayoffPlanCreator,
		SnowflakesScheduleCreator snowflakesScheduleCreator)
	{
		_debtPayoffPlanCreator =
			debtPayoffPlanCreator ?? throw new ArgumentNullException(nameof(debtPayoffPlanCreator));
		_snowflakesScheduleCreator = snowflakesScheduleCreator ??
		                             throw new ArgumentNullException(nameof(snowflakesScheduleCreator));
	}

	public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(
		List<LoanDto> debts,
		List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		Console.WriteLine(
			$"Entered function 'CalculatePaymentPlansAsync' with debts: {JsonSerializer.Serialize(debts)}," +
			$" snowflakes: {JsonSerializer.Serialize(snowflakes)}, debtPlanMonthlyPayment: {debtPlanMonthlyPayment}");

		if (!IsInputsValid(debts, snowflakes, debtPlanMonthlyPayment))
			return new DebtPayoffPlan(); // Return empty DebtPayoffPlan if inputs are not valid

		//CreateSnowflakesSchedule(debts, snowflakes, debtPlanMonthlyPayment);

		DebtPayoffPlan result = await _debtPayoffPlanCreator.CalculatePaymentPlansAsync(debts);

		Console.WriteLine($"Successfully calculated payment plans: {JsonSerializer.Serialize(result)}");

		return result;
	}

	private static bool IsInputsValid(List<LoanDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		if (debts == null || debts.Count < 1)
		{
			Console.WriteLine(
				"Debts parameter is null or has a count of zero. Exiting function 'CalculatePaymentPlansAsync'.");
			return false;
		}

		if (snowflakes == null)
		{
			Console.WriteLine("Snowflakes parameter is null. Exiting function 'CalculatePaymentPlansAsync'.");
			return false;
		}

		if (debtPlanMonthlyPayment < (decimal)0.001)
		{
			Console.WriteLine(
				$"Warning debtPlanMonthlyPayment parameter is {debtPlanMonthlyPayment}. will set it to 0.");
			debtPlanMonthlyPayment = 0;
		}

		return true;
	}

	private void CreateSnowflakesSchedule(List<LoanDto> debts, List<SnowflakeDto> snowflakes,
		decimal debtPlanMonthlyPayment)
	{
		// Determine the maximum amount of time that the schedule will last based on the remaining term in months across all debts
		int maxTime = debts.Max(d => d.RemainingTermInMonths);
		var snowflakesSchedule = _snowflakesScheduleCreator.CalculateSnowflakes(snowflakes, maxTime);
		CalculateExtraPayment(debts, debtPlanMonthlyPayment);
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
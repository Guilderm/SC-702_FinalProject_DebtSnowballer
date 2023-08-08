using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class SnowflakesScheduleCalculator
{
	public List<SnowflakesScheduleDetail> CalculateSnowflakes(List<SnowflakeDto> snowflakes, int maxTime)
	{
		var calculations = new List<SnowflakesScheduleDetail>();

		foreach (SnowflakeDto snowflake in snowflakes)
		{
			// Determine the start and end dates for this Snowflake
			DateTime startDate = snowflake.StartingAt ?? DateTime.Today;
			DateTime endDate = snowflake.EndingAt ?? DateTime.Today.AddMonths(maxTime);

			// Calculate the Snowflake amounts
			DateTime date = startDate;
			do
			{
				SnowflakesScheduleDetail existingCalculation = calculations.FirstOrDefault(c => c.Date == date);

				if (existingCalculation != null)
					// If a calculation already exists for this date, add to the existing amount
					existingCalculation.Amount += snowflake.Amount;
				else
					// If no calculation exists for this date, create a new one
					calculations.Add(new SnowflakesScheduleDetail { Date = date, Amount = snowflake.Amount });

				date = date.AddMonths(snowflake.FrequencyInMonths);
			} while (date <= endDate);
		}

		return calculations;
	}
}
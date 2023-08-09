using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class SnowflakesScheduleCalculator
{
	public List<SnowflakesScheduleDetail> CalculateSnowflakes(List<SnowflakeDto> snowflakes, int maxTime)
	{
		Console.WriteLine(
			$"Entered function 'CalculateSnowflakes' with snowflakes count: {snowflakes.Count} and maxTime: {maxTime}");

		var snowflakesSchedule = new List<SnowflakesScheduleDetail>();

		foreach (SnowflakeDto snowflake in snowflakes)
		{
			Console.WriteLine(
				$"Processing snowflake with StartDate: {snowflake.StartingAt} and EndDate: {snowflake.EndingAt}");

			// Determine the start and end dates for this Snowflake
			DateTime startDate = snowflake.StartingAt ?? DateTime.Today;
			DateTime endDate = snowflake.EndingAt ?? DateTime.Today.AddMonths(maxTime);

			DateTime date = startDate;
			do
			{
				SnowflakesScheduleDetail snowflakeEntry = snowflakesSchedule.FirstOrDefault(c => c.Date == date);

				if (snowflakeEntry == null)
				{
					// If no calculation exists for this date, create a new one
					snowflakesSchedule.Add(new SnowflakesScheduleDetail { Date = date, Amount = snowflake.Amount });
					Console.WriteLine($"Added new snowflake entry for date: {date}");
				}
				else
				{
					// If a calculation already exists for this date, add to the existing amount
					snowflakeEntry.Amount += snowflake.Amount;
					Console.WriteLine($"Updated existing snowflake entry for date: {date}");
				}

				date = date.AddMonths(snowflake.FrequencyInMonths);
			} while (date <= endDate);
		}

		Console.WriteLine($"Successfully calculated snowflakes schedule. Total entries: {snowflakesSchedule.Count}");
		return snowflakesSchedule;
	}
}
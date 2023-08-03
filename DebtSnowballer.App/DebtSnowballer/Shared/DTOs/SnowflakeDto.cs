namespace DebtSnowballer.Shared.DTOs;

public class SnowflakeDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string NickName { get; set; }
	public int FrequencyInMonths { get; set; }
	public decimal Amount { get; set; }
	public DateTime StartingAt { get; set; }
	public DateTime EndingAt { get; set; }
	public string CurrencyCode { get; set; }
}
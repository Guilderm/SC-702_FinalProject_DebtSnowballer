namespace DebtSnowballer.Shared.DTOs;

public class SnowflakeDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public int Frequency { get; set; }
	public decimal Amount { get; set; }
}
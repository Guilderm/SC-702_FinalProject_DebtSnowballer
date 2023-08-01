namespace DebtSnowballer.Shared.DTOs;

public class ExchangeRateDto
{
	public int Id { get; set; }
	public string BaseCurrency { get; set; } = null!;
	public string TargetCurrency { get; set; } = null!;
	public decimal Rate { get; set; }
	public DateTime NextUpdateTime { get; set; }
}
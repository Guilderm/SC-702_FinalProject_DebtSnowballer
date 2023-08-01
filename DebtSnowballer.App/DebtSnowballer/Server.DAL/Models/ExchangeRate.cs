namespace Server.DAL.Models;

public class ExchangeRate
{
	public int Id { get; set; }
	public string BaseCurrency { get; set; } = null!;
	public string TargetCurrency { get; set; } = null!;
	public decimal Rate { get; set; }
	public DateTime LastUpdated { get; set; }
}
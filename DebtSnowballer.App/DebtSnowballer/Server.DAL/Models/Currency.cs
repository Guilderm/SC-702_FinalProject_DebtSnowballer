namespace Server.DAL.Models;

public class Currency
{
	public Currency()
	{
		ExchangeRateBaseCurrencyNavigation = new HashSet<ExchangeRate>();
		ExchangeRateQuoteCurrencyNavigation = new HashSet<ExchangeRate>();
		LoanDetails = new HashSet<LoanDetail>();
		PlannedSnowflakes = new HashSet<PlannedSnowflake>();
	}

	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string AlphaCode { get; set; } = null!;
	public int NumericCode { get; set; }
	public string Symbol { get; set; } = null!;
	public int Precision { get; set; }

	public virtual ICollection<ExchangeRate> ExchangeRateBaseCurrencyNavigation { get; set; }
	public virtual ICollection<ExchangeRate> ExchangeRateQuoteCurrencyNavigation { get; set; }
	public virtual ICollection<LoanDetail> LoanDetails { get; set; }
	public virtual ICollection<PlannedSnowflake> PlannedSnowflakes { get; set; }
}
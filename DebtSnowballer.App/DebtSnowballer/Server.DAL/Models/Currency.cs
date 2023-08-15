namespace Server.DAL.Models;

public class Currency
{
	public Currency()
	{
		ExchangeRateBaseCurrencyNavigations = new HashSet<ExchangeRate>();
		ExchangeRateQuoteCurrencyNavigations = new HashSet<ExchangeRate>();
		LoanDetails = new HashSet<LoanDetail>();
		PlannedSnowflakes = new HashSet<PlannedSnowflake>();
		UserPreferences = new HashSet<UserPreference>();
	}

	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string AlphaCode { get; set; } = null!;
	public int NumericCode { get; set; }
	public string Symbol { get; set; } = null!;
	public int Precision { get; set; }

	public virtual ICollection<ExchangeRate> ExchangeRateBaseCurrencyNavigations { get; set; }
	public virtual ICollection<ExchangeRate> ExchangeRateQuoteCurrencyNavigations { get; set; }
	public virtual ICollection<LoanDetail> LoanDetails { get; set; }
	public virtual ICollection<PlannedSnowflake> PlannedSnowflakes { get; set; }
	public virtual ICollection<UserPreference> UserPreferences { get; set; }
}
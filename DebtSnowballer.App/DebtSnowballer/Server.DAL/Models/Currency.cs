namespace Server.DAL.Models;

public class Currency
{
	public Currency()
	{
		Debts = new HashSet<Debt>();
		ExchangeRateBaseCurrencyNavigations = new HashSet<ExchangeRate>();
		ExchangeRateQuoteCurrencyNavigations = new HashSet<ExchangeRate>();
		Snowflakes = new HashSet<Snowflake>();
	}

	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string AlphaCode { get; set; } = null!;
	public int NumericCode { get; set; }
	public string Symbol { get; set; } = null!;
	public int Precision { get; set; }

	public virtual ICollection<Debt> Debts { get; set; }
	public virtual ICollection<ExchangeRate> ExchangeRateBaseCurrencyNavigations { get; set; }
	public virtual ICollection<ExchangeRate> ExchangeRateQuoteCurrencyNavigations { get; set; }
	public virtual ICollection<Snowflake> Snowflakes { get; set; }
}
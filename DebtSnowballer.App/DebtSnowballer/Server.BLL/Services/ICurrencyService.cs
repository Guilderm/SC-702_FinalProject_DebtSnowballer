namespace Server.BLL.Services;

public interface ICurrencyService
{
	Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency);
}
namespace Server.BLL.Services;

public interface ICurrencyService
{
	Task<decimal> GetExchangeRate(string baseCurrency);
}
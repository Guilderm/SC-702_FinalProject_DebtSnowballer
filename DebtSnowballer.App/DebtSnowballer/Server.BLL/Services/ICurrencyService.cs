using Server.DAL.Models;

namespace Server.BLL.Services;

public interface ICurrencyService
{
	Task<IEnumerable<ExchangeRate>> GetExchangeRate(string baseCurrency);
}
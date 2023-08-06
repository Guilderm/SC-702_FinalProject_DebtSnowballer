using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IDebtService
{
	Task<DebtDto> AddDebt(DebtDto debtDto);
	Task DeleteDebt(int id);
	Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency();
	Task<DebtDto> GetDebtById(int id);
	Task<IList<ExchangeRateDto>> GetUsersExchangeRates(string userId);
	Task<DebtDto> UpdateDebt(DebtDto debtDto);
}
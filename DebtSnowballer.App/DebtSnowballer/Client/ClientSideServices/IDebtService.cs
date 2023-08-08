using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IDebtService
{
	Task<DebtDto> AddDebt(DebtDto debtDto);
	Task DeleteDebt(int id);
	Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency();
	Task<DebtDto> GetDebtById(int id);
	Task<DebtDto> UpdateDebt(DebtDto debtDto);
	Task<IList<ExchangeRateDto>> GetUsersExchangeRates();
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IDebtService
{
	Task<IList<DebtDto>> GetDebts();
	Task<DebtDto> GetDebtById(int id);
	Task DeleteDebt(int id);
	Task<DebtDto> AddDebt(DebtDto debtDto);
	Task<DebtDto> UpdateDebt(DebtDto debtDto);
	Task<IList<DebtDto>> GetAllDebtsInBaseCurrency();
}
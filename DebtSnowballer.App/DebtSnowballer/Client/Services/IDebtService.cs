using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IDebtService
{
	Task<IList<DebtDto>> GetDebts();
	Task<DebtDto> GetDebtById(int id);
	Task DeleteItem(int id);
	Task<DebtDto> AddItem(DebtDto debtDto);
	Task<DebtDto> UpdateItem(DebtDto debtDto);
}
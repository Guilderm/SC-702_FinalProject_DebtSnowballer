using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IDebtService
{
	Task<IList<DebtDto>> GetDebtbyAuth0UserId(string auth0UserId);
	Task<DebtDto> GetDebtByIdAndAuth0UserId(int id, string auth0UserId);
	Task DeleteItem(int id);
	Task<DebtDto> PostDebt(DebtDto debtDto);
}
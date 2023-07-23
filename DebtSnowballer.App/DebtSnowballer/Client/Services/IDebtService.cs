using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface IDebtService
{
	Task<IList<DebtDto>> GetDebtbyAuth0UserId(string Auth0SUD);
	Task<DebtDto> GetItem(int id);
	Task DeleteItem(int id);
}
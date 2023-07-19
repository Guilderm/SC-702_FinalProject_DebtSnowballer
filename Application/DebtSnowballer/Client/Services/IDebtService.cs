using System.Collections.Generic;
using System.Threading.Tasks;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services
{
	public interface IDebtService
	{
		Task<IList<DebtDto>> GetDebtbySUD(string Auth0SUD);
		Task<DebtDto> GetItem(int id);
		Task DeleteItem(int id);
	}
}
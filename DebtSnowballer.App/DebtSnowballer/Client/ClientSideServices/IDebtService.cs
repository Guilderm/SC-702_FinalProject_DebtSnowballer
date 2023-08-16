using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IDebtService
{
	Task<LoanDetailDto> AddDebt(LoanDetailDto loanDetailDto);
	Task DeleteLoan(int id);
	Task<IList<LoanDetailDto>> GetAllDebtsInQuoteCurrency();
	Task<LoanDetailDto> GetDebtById(int id);
	Task<LoanDetailDto> UpdateDebt(LoanDetailDto loanDetailDto);
	Task<IList<ExchangeRateDto>> GetUsersExchangeRates();
}
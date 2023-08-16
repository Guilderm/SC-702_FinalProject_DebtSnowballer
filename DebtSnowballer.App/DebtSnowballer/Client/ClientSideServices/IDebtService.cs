using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IDebtService
{
	Task<LoanDetailDto> CreateLoan(LoanDetailDto loan);
	Task DeleteLoan(int id);
	Task<IList<LoanDetailDto>> GetAllDebtsInQuoteCurrency();
	Task<LoanDetailDto> UpdateLoan(LoanDetailDto loanDetailDto);
	Task<IList<ExchangeRateDto>> GetUsersExchangeRates();
}
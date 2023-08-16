using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IDebtService
{
	Task<LoanDto> CreateLoan(LoanDto loan);
	Task DeleteLoan(int id);
	Task<IList<LoanDto>> GetAllDebtsInQuoteCurrency();
	Task<LoanDto> UpdateLoan(LoanDto loanDto);
	Task<IList<ExchangeRateDto>> GetUsersExchangeRates();
}
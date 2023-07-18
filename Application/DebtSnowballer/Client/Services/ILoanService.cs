using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface ILoanService
{
	Task<List<LoanDto>> GetLoans();
	Task<LoanDto> GetLoan(int id);
	Task<LoanDto> AddLoan(LoanDto loanDto);
	Task<LoanDto> UpdateLoan(LoanDto loanDto);
	Task DeleteLoan(int id);
}
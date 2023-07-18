using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class LoanService : ILoanService
{
	private readonly HttpClient _httpClient;

	public LoanService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<LoanDto>> GetLoans()
	{
		return await _httpClient.GetFromJsonAsync<List<LoanDto>>("api/Loan");
	}

	public async Task<LoanDto> GetLoan(int id)
	{
		return await _httpClient.GetFromJsonAsync<LoanDto>($"api/Loan/{id}");
	}

	public async Task<LoanDto> AddLoan(LoanDto loanDto)
	{
		var response = await _httpClient.PostAsJsonAsync("api/Loan", loanDto);
		return await response.Content.ReadFromJsonAsync<LoanDto>();
	}

	public async Task<LoanDto> UpdateLoan(LoanDto loanDto)
	{
		var response = await _httpClient.PutAsJsonAsync($"api/Loan/{loanDto.Id}", loanDto);
		return await response.Content.ReadFromJsonAsync<LoanDto>();
	}


	public async Task DeleteLoan(int id)
	{
		await _httpClient.DeleteAsync($"api/Loan/{id}");
	}
}
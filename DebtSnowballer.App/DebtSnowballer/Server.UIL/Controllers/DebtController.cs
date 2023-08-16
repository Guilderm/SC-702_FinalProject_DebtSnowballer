using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerSideServices;

namespace Server.UIL.Controllers;

public class DebtController : BaseController
{
	private readonly DebtManagement _debtManagement;
	private readonly ExchangeRateManagement _exchangeRateManagement;
	private readonly ILogger<DebtController> _logger;

	public DebtController(DebtManagement debtManagement, ILogger<DebtController> logger,
		ExchangeRateManagement exchangeRateManagement)
		: base(logger)
	{
		_logger = logger;
		_debtManagement = debtManagement;
		_exchangeRateManagement = exchangeRateManagement;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] LoanDto loan)
	{
		if (!ModelState.IsValid)
		{
			var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
			var errorString = string.Join("; ", errors);
			_logger.LogWarning("Invalid request data for creating a Loan. Errors: {errors}", errorString);
			return BadRequest(ModelState);
		}

		try
		{
			string? auth0UserId = GetAuth0UserId();
			_logger.LogInformation("Creating debt for user {userId}", auth0UserId);

			LoanDto createdLoan = await _debtManagement.CreateNewLoan(loan, auth0UserId);

			_logger.LogInformation("Debt created with id {debtId}", createdLoan.Id);

			return CreatedAtAction(nameof(Get), new { id = createdLoan.Id }, createdLoan);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while creating debt for user {userId}", GetAuth0UserId());
			return BadRequest(new { message = "An error occurred while creating the debt. Please try again." });
		}
	}


	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] LoanDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Updating debt with id {debtId} for user {userId}", id, auth0UserId);

		LoanDto updatedLoan = await _debtManagement.UpdateDebt(id, requestDto, auth0UserId);

		_logger.LogInformation("Debt with id {debtId} updated for user {userId}", id, auth0UserId);

		return Ok(updatedLoan);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Deleting debt with id {debtId} for user {userId}", id, auth0UserId);

		await _debtManagement.DeleteDebt(id, auth0UserId);

		_logger.LogInformation("Debt with id {debtId} deleted for user {userId}", id, auth0UserId);

		return NoContent();
	}

	[HttpGet("GetAllDebtsInQuoteCurrency")]
	public async Task<IActionResult> GetAllDebtsInQuoteCurrency()
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Fetching all debts in quote currency for user {userId}", auth0UserId);

		IList<LoanDto> debts = await _debtManagement.GetAllDebtsInQuoteCurrency(auth0UserId);

		_logger.LogInformation("Fetched {count} debts in quote currency for user {userId}", debts.Count, auth0UserId);

		return Ok(debts);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id)
	{
		string? auth0UserId = GetAuth0UserId();
		_logger.LogInformation("Fetching debt with id {debtId} for user {userId}", id, auth0UserId);

		LoanDto loan = await _debtManagement.GetDebt(id, auth0UserId);

		_logger.LogInformation("Fetched debt with id {debtId} for user {userId}", id, auth0UserId);

		return Ok(loan);
	}

	[HttpGet("GetUsersExchangeRates")]
	public async Task<IActionResult> GetUsersExchangeRates()
	{
		string? auth0UserId = GetAuth0UserId();

		_logger.LogInformation("Fetching exchange rates for user {userId}", auth0UserId);

		IEnumerable<ExchangeRateDto> exchangeRates = await _exchangeRateManagement.GetUsersExchangeRates(auth0UserId);

		_logger.LogInformation("Fetched {count} exchange rates for user {userId}", exchangeRates.Count(), auth0UserId);

		return Ok(exchangeRates);
	}
}
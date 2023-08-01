using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

public class DebtController : BaseController
{
	private readonly DebtManagement _debtManagement;
	private readonly ILogger<DebtController> _logger;

	public DebtController(DebtManagement debtManagement, ILogger<DebtController> logger)
		: base(logger)
	{
		_debtManagement = debtManagement;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] DebtDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		DebtDto createdDebt = await _debtManagement.CreateDebt(requestDto, auth0UserId);

		return CreatedAtAction(nameof(Get), new { id = createdDebt.Id }, createdDebt);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] DebtDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		DebtDto updatedDebt = await _debtManagement.UpdateDebt(id, requestDto, auth0UserId);

		return Ok(updatedDebt);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		string? auth0UserId = GetAuth0UserId();

		await _debtManagement.DeleteDebt(id, auth0UserId);

		return NoContent();
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		string? auth0UserId = GetAuth0UserId();

		IList<DebtDto> debts = await _debtManagement.GetAllDebts(auth0UserId);

		return Ok(debts);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id)
	{
		string? auth0UserId = GetAuth0UserId();

		DebtDto debt = await _debtManagement.GetDebt(id, auth0UserId);

		return Ok(debt);
	}

	[HttpGet("GetAllDebtsInBaseCurrency")]
	public async Task<IActionResult> GetAllDebtsInBaseCurrency()
	{
		try
		{
			var debtsInBaseCurrency = await _debtManagement.GetAllDebtsInBaseCurrency(GetAuth0UserId());

			if (debtsInBaseCurrency == null) return NotFound();

			return Ok(debtsInBaseCurrency);
		}
		catch (Exception ex)
		{
			// Log the exception message
			_logger.LogError(ex, $"Something went wrong in the {nameof(GetAllDebtsInBaseCurrency)}");
			return StatusCode(500, "Internal server error");
		}
	}
}
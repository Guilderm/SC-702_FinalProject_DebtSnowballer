using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

public class DebtController : BaseController
{
	private readonly DebtManagement _debtManagement;

	public DebtController(DebtManagement debtManagement, ILogger<DebtController> logger)
		: base(logger)
	{
		_debtManagement = debtManagement;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] DebtDto requestDto)
	{
		var auth0UserId = GetAuth0UserId();

		DebtDto createdDebt = await _debtManagement.CreateDebt(requestDto, auth0UserId);

		return CreatedAtAction(nameof(Get), new { id = createdDebt.Id }, createdDebt);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] DebtDto requestDto)
	{
		var auth0UserId = GetAuth0UserId();

		DebtDto updatedDebt = await _debtManagement.UpdateDebt(id, requestDto, auth0UserId);

		return Ok(updatedDebt);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var auth0UserId = GetAuth0UserId();

		await _debtManagement.DeleteDebt(id, auth0UserId);

		return NoContent();
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var auth0UserId = GetAuth0UserId();

		IList<DebtDto> debts = await _debtManagement.GetAllDebts(auth0UserId);

		return Ok(debts);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id)
	{
		var auth0UserId = GetAuth0UserId();

		DebtDto debt = await _debtManagement.GetDebt(id, auth0UserId);

		return Ok(debt);
	}
}
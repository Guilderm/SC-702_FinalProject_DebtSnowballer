using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.Services;
using System.Security.Claims;

namespace Server.UIL.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DebtController : ControllerBase
{
	private readonly DebtManagement _debtManagement;
	private readonly ILogger<DebtController> _logger;

	public DebtController(DebtManagement debtManagement, ILogger<DebtController> logger)
	{
		_debtManagement = debtManagement;
		_logger = logger;
	}

	[HttpPost]
	public IActionResult Post([FromBody] DebtDto requestDto, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received POST request in {ControllerName} with DTO: {RequestDto}",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid POST attempt in {ControllerName} with DTO: {RequestDto}", nameof(DebtController),
				requestDto);
			return BadRequest(ModelState);
		}

		DebtDto createdDebt = _debtManagement.CreateDebt(requestDto).Result;

		_logger.LogInformation("Successfully created entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			createdDebt.Id);

		return CreatedAtAction(nameof(Get), new { id = createdDebt.Id }, createdDebt);
	}

	[HttpPut("{id:int}")]
	public IActionResult Put(int id, [FromBody] DebtDto requestDto, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received PUT request in {ControllerName} with DTO: {RequestDto}",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid PUT attempt in {ControllerName} with DTO: {RequestDto}", nameof(DebtController),
				requestDto);
			return BadRequest(ModelState);
		}

		DebtDto updatedDebt = _debtManagement.UpdateDebt(id, requestDto).Result;

		_logger.LogInformation("Successfully updated entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			updatedDebt.Id);

		return Ok(updatedDebt);
	}

	[HttpDelete("{id:int}")]
	public IActionResult Delete(int id, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received DELETE request in {ControllerName} with ID: {Id}", nameof(DebtController), id);

		_debtManagement.DeleteDebt(id);

		_logger.LogInformation("Successfully deleted entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			id);

		return NoContent();
	}

	[HttpGet("{auth0UserId}")]
	public IActionResult Get(string auth0UserId)
	{
		_logger.LogInformation("Received GET request in {ControllerName}", nameof(DebtController));

		_logger.LogError($"Log Claims");
		foreach (var claim in User.Claims)
		{
			_logger.LogError($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
		}


		if (string.IsNullOrWhiteSpace(auth0UserId))
		{
			_logger.LogError("Invalid auth0UserId: {Auth0UserId}", auth0UserId);
			return BadRequest("Invalid auth0UserId");
		}

		IList<DebtDto> debts = _debtManagement.GetAllDebts(auth0UserId).Result;

		_logger.LogInformation("Exiting GET request in {ControllerName} with mappedResult: {MappedResult}", "GET",
			nameof(DebtController), debts);

		return Ok(debts);
	}

	[HttpGet("{id:int}/{auth0UserId}")]
	public IActionResult Get(int id, string auth0UserId)
	{
		_logger.LogInformation("Received GET request in {ControllerName} with ID: {Id}", nameof(DebtController), id);

		_logger.LogError($"Log Claims");
		foreach (var claim in User.Claims)
		{
			_logger.LogError($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
		}

		if (string.IsNullOrWhiteSpace(auth0UserId))
		{
			_logger.LogError("Invalid auth0UserId: {Auth0UserId}", auth0UserId);
			return BadRequest("Invalid auth0UserId");
		}

		DebtDto debt = _debtManagement.GetDebt(id, auth0UserId).Result;

		_logger.LogInformation("Exiting GET request in {ControllerName} with mappedResult: {MappedResult}", "GET",
			nameof(DebtController), debt);

		return Ok(debt);
	}
}
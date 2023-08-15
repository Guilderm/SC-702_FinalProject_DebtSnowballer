using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.BLL.ServerSideServices;

namespace Server.UIL.Controllers;

public class SnowflakeController : BaseController
{
	private readonly ILogger<SnowflakeController> _logger;
	private readonly SnowflakeManagement _snowflakeManagement;

	public SnowflakeController(SnowflakeManagement snowflakeManagement, ILogger<SnowflakeController> logger)
		: base(logger)
	{
		_snowflakeManagement = snowflakeManagement;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] PlannedSnowflakeDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		PlannedSnowflakeDto createdPlannedSnowflake =
			await _snowflakeManagement.CreateSnowflake(requestDto, auth0UserId);

		return CreatedAtAction(nameof(Get), new { id = createdPlannedSnowflake.Id }, createdPlannedSnowflake);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] PlannedSnowflakeDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		PlannedSnowflakeDto updatedPlannedSnowflake =
			await _snowflakeManagement.UpdateSnowflake(id, requestDto, auth0UserId);

		return Ok(updatedPlannedSnowflake);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		string? auth0UserId = GetAuth0UserId();

		await _snowflakeManagement.DeleteSnowflake(id, auth0UserId);

		return NoContent();
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		string? auth0UserId = GetAuth0UserId();

		IList<PlannedSnowflakeDto> snowflakes = await _snowflakeManagement.GetAllSnowflakes(auth0UserId);

		return Ok(snowflakes);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id)
	{
		string? auth0UserId = GetAuth0UserId();

		PlannedSnowflakeDto plannedSnowflake = await _snowflakeManagement.GetSnowflake(id, auth0UserId);

		return Ok(plannedSnowflake);
	}
}
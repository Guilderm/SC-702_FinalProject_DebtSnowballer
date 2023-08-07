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
	public async Task<IActionResult> Post([FromBody] SnowflakeDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		SnowflakeDto createdSnowflake = await _snowflakeManagement.CreateSnowflake(requestDto, auth0UserId);

		return CreatedAtAction(nameof(Get), new { id = createdSnowflake.Id }, createdSnowflake);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] SnowflakeDto requestDto)
	{
		string? auth0UserId = GetAuth0UserId();

		SnowflakeDto updatedSnowflake = await _snowflakeManagement.UpdateSnowflake(id, requestDto, auth0UserId);

		return Ok(updatedSnowflake);
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

		IList<SnowflakeDto> snowflakes = await _snowflakeManagement.GetAllSnowflakes(auth0UserId);

		return Ok(snowflakes);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id)
	{
		string? auth0UserId = GetAuth0UserId();

		SnowflakeDto snowflake = await _snowflakeManagement.GetSnowflake(id, auth0UserId);

		return Ok(snowflake);
	}
}
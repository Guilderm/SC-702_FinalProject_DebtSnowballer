using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Server.BLL.Services;

namespace Server.UIL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrudController : ControllerBase
	{
	private readonly ILogger<CrudController> _logger;
	private readonly CrudManagement _crudManagement;

	public CrudController(CrudManagement crudManagement, ILogger<CrudController> logger)
		{
		_crudManagement = crudManagement;
		_logger = logger;
		}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] CrudDto crudDto)
		{
		_logger.LogInformation("Received POST request in {ControllerName} with DTO: {Dto}.", nameof(CrudController), crudDto);

		if (!ModelState.IsValid)
			{
			_logger.LogError("Invalid POST attempt in {ControllerName}.", nameof(CrudController));
			return BadRequest(ModelState);
			}

		CrudDto createdCrudDto = await _crudManagement.CreateCrud(crudDto);

		_logger.LogInformation("Created new Crud item with ID {CrudId} in {ControllerName}.", createdCrudDto.Id, nameof(CrudController));

		return CreatedAtAction(nameof(GetCrud), new { id = createdCrudDto.Id }, createdCrudDto);
		}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] CrudDto crudDto)
		{
		_logger.LogInformation("Received PUT request in {ControllerName} with ID: {Id} and DTO: {Dto}.", nameof(CrudController), id, crudDto);

		if (!ModelState.IsValid || id < 1)
			{
			_logger.LogError("Invalid UPDATE attempt in {ControllerName}.", nameof(CrudController));
			return BadRequest(ModelState);
			}

		CrudDto updatedCrudDto = await _crudManagement.UpdateCrud(id, crudDto);

		_logger.LogInformation("Updated Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

		return NoContent();
		}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
		{
		_logger.LogInformation("Received DELETE request in {ControllerName} with ID: {Id}.", nameof(CrudController), id);

		await _crudManagement.DeleteCrud(id);

		_logger.LogInformation("Deleted Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

		return NoContent();
		}

	[HttpGet]
	public async Task<IActionResult> GetCruds()
		{
		_logger.LogInformation("Received GET request in {ControllerName}.", nameof(CrudController));

		var results = await _crudManagement.GetAllCruds();

		_logger.LogInformation("Returned {Count} Crud items from {ControllerName}.", results.Count, nameof(CrudController));

		return Ok(results);
		}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetCrud(int id)
		{
		_logger.LogInformation("Received GET request in {ControllerName} with ID: {Id}.", nameof(CrudController), id);

		CrudDto result = await _crudManagement.GetCrud(id);

		_logger.LogInformation("Returned Crud item with ID {CrudId} from {ControllerName}.", id, nameof(CrudController));

		return Ok(result);
		}
	}

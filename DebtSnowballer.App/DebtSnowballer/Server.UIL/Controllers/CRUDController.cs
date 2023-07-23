using System.Text.Json;
using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.UIL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrudController : ControllerBase
{
	private readonly ILogger<CrudController> _logger;
	protected readonly IMapper Mapper;
	protected readonly IGenericRepository<Crud> Repository;
	protected readonly IUnitOfWork UnitOfWork;

	public CrudController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CrudController> logger)
	{
		UnitOfWork = unitOfWork;
		Repository = UnitOfWork.GetRepository<Crud>();
		Mapper = mapper;
		_logger = logger;
	}

	#region POST|Create - Used to create a new resource.

	[HttpPost]
	public IActionResult Post([FromBody] CrudDto requestDto)
	{
		_logger.LogInformation("Received POST request in {ControllerName} with DTO: {Dto}.", nameof(CrudController),
			JsonSerializer.Serialize(requestDto));

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid POST attempt in {ControllerName}. ModelState: {ModelState}.",
				nameof(CrudController), JsonSerializer.Serialize(ModelState));
			return BadRequest(ModelState);
		}

		var mappedResult = Mapper.Map<Crud>(requestDto);

		Repository.Insert(mappedResult);
		UnitOfWork.SaveChanges();

		_logger.LogInformation("Created new Crud item with ID {CrudId} in {ControllerName}.", mappedResult.Id,
			nameof(CrudController));

		return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
	}

	#endregion

	#region PUT|Update - Used to update an existing resource.

	[HttpPut("{id:int}")]
	public IActionResult Put(int id, [FromBody] CrudDto requestDto)
	{
		_logger.LogInformation("Received PUT request in {ControllerName} with ID: {Id} and DTO: {Dto}.",
			nameof(CrudController), id, JsonSerializer.Serialize(requestDto));

		if (!ModelState.IsValid)
		{
			var errors = ModelState
				.SelectMany(m => m.Value.Errors)
				.Select(e => e.ErrorMessage)
				.ToList();

			// Create a formatted string of all the error messages
			var errorMessages = string.Join("; ", errors);

			// Log the error with a clear message, including the controller name, the serialized ModelState, and the error messages
			_logger.LogError(
				"Invalid PUT attempt in {ControllerName}. ModelState: {ModelState}. Error Messages: {ErrorMessages}.",
				nameof(CrudController), JsonSerializer.Serialize(ModelState), errorMessages);

			return BadRequest(ModelState);
		}

		// Get the existing entity from the database
		var existingEntity = Repository.Get(id);
		if (existingEntity == null)
			// If the entity does not exist, return a 404 Not Found status
			return NotFound();

		// Map the request DTO to the existing entity
		Mapper.Map(requestDto, existingEntity);

		// Update the entity in the database
		Repository.Update(existingEntity);
		UnitOfWork.SaveChanges();

		// Map the updated entity back to a DTO to return in the response
		var updatedDto = Mapper.Map<CrudDto>(existingEntity);

		_logger.LogInformation("Updated Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

		return Ok(updatedDto);
	}

	#endregion

	#region PATCH|Update - Used to partially update an existing resource.

	[HttpPatch]
	public IActionResult Patch()
	{
		_logger.LogInformation("Received PATCH request in {ControllerName}.", nameof(CrudController));
		_logger.LogError("PATCH method is not implemented in {ControllerName}.", nameof(CrudController));
		throw new NotImplementedException();
	}

	#endregion

	#region DELETE|Delete - Used to delete a resource.

	[HttpDelete("{id:int}")]
	public IActionResult Delete(int id)
	{
		_logger.LogInformation("Received DELETE request in {ControllerName} with ID: {Id}.", nameof(CrudController),
			id);

		var dbResult = Repository.Get(id);
		Repository.Remove(dbResult);
		UnitOfWork.SaveChanges();

		_logger.LogInformation("Deleted Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

		return NoContent();
	}

	#endregion

	#region GET|Read - Used to retrieve a resource or a collection of resources.

	[HttpGet]
	public IActionResult Get()
	{
		_logger.LogInformation("Received GET request in {ControllerName}.", nameof(CrudController));

		var dbResult = Repository.GetAll();
		var mappedResult = Mapper.Map<IList<CrudDto>>(dbResult);

		_logger.LogInformation("Returned {Count} Crud items from {ControllerName}.", mappedResult.Count,
			nameof(CrudController));

		return Ok(mappedResult);
	}

	[HttpGet("{id:int}")]
	public IActionResult Get(int id)
	{
		_logger.LogInformation("Received GET request in {ControllerName} with ID: {Id}.", nameof(CrudController), id);

		var dbResult = Repository.Get(id);
		var mappedResult = Mapper.Map<CrudDto>(dbResult);

		_logger.LogInformation("Returned Crud item with ID {CrudId} from {ControllerName}.", id,
			nameof(CrudController));

		return Ok(mappedResult);
	}

	#endregion
}
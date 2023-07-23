using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.UIL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebtController : ControllerBase
{
	private readonly ILogger<DebtController> _logger;
	protected readonly IMapper Mapper;
	protected readonly IGenericRepository<Debt> Repository;
	protected readonly IUnitOfWork UnitOfWork;

	public DebtController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DebtController> logger)
	{
		UnitOfWork = unitOfWork;
		Repository = UnitOfWork.GetRepository<Debt>();
		Mapper = mapper;
		_logger = logger;
	}

	#region POST|Create - Used to create a new resource.

	[HttpPost]
	public IActionResult Post([FromBody] DebtDto requestDto)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with DTO: {RequestDto}", "POST",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid {HttpMethod} attempt in {ControllerName} with DTO: {RequestDto}", "POST",
				nameof(DebtController), requestDto);
			return BadRequest(ModelState);
		}

		var mappedResult = Mapper.Map<Debt>(requestDto);

		Repository.Insert(mappedResult);
		UnitOfWork.SaveChanges();

		_logger.LogInformation("Successfully created entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			mappedResult.Id);

		return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
	}

	#endregion

	#region PUT|Update - Used to update an existing resource.

	[HttpPut("{id:int}")]
	public IActionResult Put(int id, [FromBody] DebtDto requestDto)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with DTO: {RequestDto}", "PUT",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid {HttpMethod} attempt in {ControllerName} with DTO: {RequestDto}", "PUT",
				nameof(DebtController), requestDto);
			return BadRequest(ModelState);
		}

		var existingEntity = Repository.Get(id);
		if (existingEntity == null)
		{
			_logger.LogError("Entity not found in {ControllerName} with ID: {Id}", nameof(DebtController), id);
			return NotFound();
		}

		Mapper.Map(requestDto, existingEntity);

		Repository.Update(existingEntity);
		UnitOfWork.SaveChanges();

		var updatedDto = Mapper.Map<DebtDto>(existingEntity);

		_logger.LogInformation("Successfully updated entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			existingEntity.Id);

		return Ok(updatedDto);
	}

	#endregion

	#region PATCH|Update - Used to partially update an existing resource.

	[HttpPatch]
	public IActionResult Patch()
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName}", "PATCH", nameof(DebtController));
		throw new NotImplementedException();
	}

	#endregion

	#region DELETE|Delete - Used to delete a resource.

	[HttpDelete("{id:int}")]
	public IActionResult Delete(int id)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with ID: {Id}", "DELETE",
			nameof(DebtController), id);

		var dbResult = Repository.Get(id);
		Repository.Remove(dbResult);
		UnitOfWork.SaveChanges();

		_logger.LogInformation("Successfully deleted entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			id);

		return NoContent();
	}

	#endregion

	#region GET|Read - Used to retrieve a resource or a collection of resources.

	[HttpGet]
	public IActionResult Get()
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName}", "GET", nameof(DebtController));

		var dbResult = Repository.GetAll();
		var mappedResult = Mapper.Map<IList<DebtDto>>(dbResult);

		_logger.LogInformation("Exiting {HttpMethod} request in {ControllerName} with mappedResult: {MappedResult}",
			"GET", nameof(DebtController), mappedResult);

		return Ok(mappedResult);
	}

	[HttpGet("{id:int}")]
	public IActionResult Get(int id)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with ID: {Id}", "GET",
			nameof(DebtController), id);

		var dbResult = Repository.Get(id);
		var mappedResult = Mapper.Map<DebtDto>(dbResult);

		_logger.LogInformation("Exiting {HttpMethod} request in {ControllerName} with mappedResult: {MappedResult}",
			"GET", nameof(DebtController), mappedResult);

		return Ok(mappedResult);
	}

	#endregion
}
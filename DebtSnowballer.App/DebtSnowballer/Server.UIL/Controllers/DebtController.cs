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
	protected readonly IMapper _mapper;
	private readonly IGenericRepository<Debt> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DebtController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DebtController> logger)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_logger = logger;
		_repository = _unitOfWork.DebtRepository;
	}

	#region POST|Create - Used to create a new resource.

	[HttpPost]
	public IActionResult Post([FromBody] DebtDto requestDto, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with DTO: {RequestDto}", "POST",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid {HttpMethod} attempt in {ControllerName} with DTO: {RequestDto}", "POST",
				nameof(DebtController), requestDto);
			return BadRequest(ModelState);
		}

		Debt? mappedResult = _mapper.Map<Debt>(requestDto);
		_repository.Insert(mappedResult);
		_unitOfWork.Save();

		_logger.LogInformation("Successfully created entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			mappedResult.Id);

		return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
	}

	#endregion

	#region PUT|Update - Used to update an existing resource.

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Put(int id, [FromBody] DebtDto requestDto, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with DTO: {RequestDto}", "PUT",
			nameof(DebtController), requestDto);

		if (!ModelState.IsValid)
		{
			_logger.LogError("Invalid {HttpMethod} attempt in {ControllerName} with DTO: {RequestDto}", "PUT",
				nameof(DebtController), requestDto);
			return BadRequest(ModelState);
		}

		Debt? existingEntity = await _repository.Get(d => d.Id == id);
		if (existingEntity == null)
		{
			_logger.LogError("Entity not found in {ControllerName} with ID: {Id}", nameof(DebtController), id);
			return NotFound();
		}

		_mapper.Map(requestDto, existingEntity);
		_repository.Update(existingEntity);
		await _unitOfWork.Save();

		DebtDto? updatedDto = _mapper.Map<DebtDto>(existingEntity);

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
		_logger.LogError("Received {HttpMethod} is not implemented in {ControllerName}.", "PATCH",
			nameof(DebtController));
		throw new NotImplementedException();
	}

	#endregion

	#region DELETE|Delete - Used to delete a resource.

	[HttpDelete("{id:int}")]
	public IActionResult Delete(int id, [FromHeader] string auth0UserId)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with ID: {Id}", "DELETE",
			nameof(DebtController), id);

		Task<Debt>? dbResult = _repository.Get(d => d.Id == id);
		if (dbResult == null)
		{
			_logger.LogError("Entity not found in {ControllerName} with ID: {Id}", nameof(DebtController), id);
			return NotFound();
		}

		_repository.Delete(id); // Changed from _repository.Remove(dbResult);
		_unitOfWork.Save();

		_logger.LogInformation("Successfully deleted entity in {ControllerName} with ID: {Id}", nameof(DebtController),
			id);

		return NoContent();
	}

	#endregion

	#region GET|Read - Used to retrieve a resource or a collection of resources.

	[HttpGet("{auth0UserId}")]
	public IActionResult Get(string auth0UserId)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName}", "GET", nameof(DebtController));
		Task<IList<Debt>>? dbResult = _repository.GetAll();

		if (dbResult == null)
		{
			_logger.LogWarning("No data found for auth0UserId: {Auth0UserId}", auth0UserId);
			return NotFound(); // or return NoContent();
		}

		IList<DebtDto>? mappedResult = _mapper.Map<IList<DebtDto>>(dbResult);
		_logger.LogInformation("Exiting {HttpMethod} request in {ControllerName} with mappedResult: {MappedResult}",
			"GET", nameof(DebtController), mappedResult);
		return Ok(mappedResult);
	}

	[HttpGet("{id:int}/{auth0UserId}")]
	public async Task<IActionResult> Get(int id, string auth0UserId)
	{
		_logger.LogInformation("Received {HttpMethod} request in {ControllerName} with ID: {Id}", "GET",
			nameof(DebtController), id);

		Debt? dbResult = await _repository.Get(d => d.Id == id);
		if (dbResult == null)
		{
			_logger.LogError("Entity not found in {ControllerName} with ID: {Id}", nameof(DebtController), id);
			return NotFound();
		}

		DebtDto? mappedResult = _mapper.Map<DebtDto>(dbResult);

		_logger.LogInformation("Exiting {HttpMethod} request in {ControllerName} with mappedResult: {MappedResult}",
			"GET", nameof(DebtController), mappedResult);

		return Ok(mappedResult);
	}

	#endregion
}
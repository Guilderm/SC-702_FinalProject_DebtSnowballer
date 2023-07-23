using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using System.Text.Json;

namespace Server.UIL.Controllers
{
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
			_logger.LogInformation($"Attempting to create a new Crud item with name {nameof(requestDto)}.");

			if (!ModelState.IsValid)
			{
				_logger.LogError($"Invalid POST attempt in {nameof(requestDto)}");
				return BadRequest(ModelState);
			}

			var mappedResult = Mapper.Map<Crud>(requestDto);

			Repository.Insert(mappedResult);
			UnitOfWork.SaveChanges();

			_logger.LogCritical($"The ID of Crud item with name {nameof(requestDto)} is {mappedResult.Id} .");

			return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
		}

		#endregion

		#region PUT|Update - Used to update an existing resource.

		[HttpPut("{id:int}")]
		public IActionResult Put(int id, [FromBody] TModel requestDto)
			{
			_logger.LogInformation(
				$"Received POST request in {nameof(GenericControllers<TEntity, TModel>)} with DTO: {JsonSerializer.Serialize(requestDto)}");

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
					$"Invalid POST attempt in {nameof(DebtController)}. The model state is invalid. ModelState: {JsonSerializer.Serialize(ModelState)}. Error Messages: {errorMessages}");

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
			var updatedDto = Mapper.Map<TModel>(existingEntity);

			return Ok(updatedDto);
			}

		#endregion

		#region PATCH|Update - Used to partially update an existing resource.

		[HttpPatch]
		public IActionResult Patch()
			{
			throw new NotImplementedException();
			}

		#endregion

		#region DELETE|Delete - Used to delete a resource.

		[HttpDelete("{id:int}")]
		public IActionResult Delete(int id)
			{
			var dbResult = Repository.Get(id);
			Repository.Remove(dbResult);
			UnitOfWork.SaveChanges();

			return NoContent();
			}

		#endregion

		#region GET|Read - Used to retrieve a resource or a collection of resources.

		[HttpGet]
		public IActionResult Get()
			{
			var dbResult = Repository.GetAll();
			var mappedResult = Mapper.Map<IList<TModel>>(dbResult);
			return Ok(mappedResult);
			}

		[HttpGet("{id:int}")]
		public IActionResult Get(int id)
			{
			var dbResult = Repository.Get(id);
			var mappedResult = Mapper.Map<TModel>(dbResult);
			return Ok(mappedResult);
			}

		#endregion
		}
	}
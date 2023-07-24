using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using DebtSnowballer.Shared.DTOs;
using System.Threading.Tasks;

namespace Server.UIL.Controllers
	{
	[ApiController]
	[Route("api/[controller]")]
	public class CrudController : ControllerBase
		{
		private readonly ILogger<CrudController> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<Crud> _repository;

		public CrudController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CrudController> logger)
			{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
			_repository = unitOfWork.CrudRepository;
			}

		#region GET|Read - Used to retrieve a resource or a collection of resources.

		[HttpGet]
		public async Task<IActionResult> GetCruds()
			{
			_logger.LogInformation("Received GET request in {ControllerName}.", nameof(CrudController));

			var cruds = await _repository.GetAll();
			var results = _mapper.Map<IList<CrudDto>>(cruds);

			_logger.LogInformation("Returned {Count} Crud items from {ControllerName}.", results.Count, nameof(CrudController));

			return Ok(results);
			}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCrud(int id)
			{
			_logger.LogInformation("Received GET request in {ControllerName} with ID: {Id}.", nameof(CrudController), id);

			var crud = await _repository.Get(id);
			var result = _mapper.Map<CrudDto>(crud);

			_logger.LogInformation("Returned Crud item with ID {CrudId} from {ControllerName}.", id, nameof(CrudController));

			return Ok(result);
			}

		#endregion

		#region POST|Create - Used to create a new resource.

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] CrudDto crudDto)
			{
			_logger.LogInformation("Received POST request in {ControllerName} with DTO: {Dto}.", nameof(CrudController), crudDto);

			if (!ModelState.IsValid)
				{
				_logger.LogError("Invalid POST attempt in {ControllerName}.", nameof(CrudController));
				return BadRequest(ModelState);
				}

			var crud = _mapper.Map<Crud>(crudDto);
			await _repository.Insert(crud);
			await _unitOfWork.Save();

			_logger.LogInformation("Created new Crud item with ID {CrudId} in {ControllerName}.", crud.Id, nameof(CrudController));

			return CreatedAtAction(nameof(GetCrud), new { id = crud.Id }, crud);
			}

		#endregion

		#region PUT|Update - Used to update an existing resource.

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Put(int id, [FromBody] CrudDto crudDto)
			{
			_logger.LogInformation("Received PUT request in {ControllerName} with ID: {Id} and DTO: {Dto}.", nameof(CrudController), id, crudDto);

			if (!ModelState.IsValid || id < 1)
				{
				_logger.LogError("Invalid UPDATE attempt in {ControllerName}.", nameof(CrudController));
				return BadRequest(ModelState);
				}

			var crud = await _repository.Get(id);
			if (crud == null)
				{
				_logger.LogError("Invalid UPDATE attempt in {ControllerName}.", nameof(CrudController));
				return BadRequest("Submitted data is invalid");
				}

			_mapper.Map(crudDto, crud);
			_repository.Update(crud);
			await _unitOfWork.Save();

			_logger.LogInformation("Updated Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

			return NoContent();
			}

		#endregion

		#region DELETE|Delete - Used to delete a resource.

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
			{
			_logger.LogInformation("Received DELETE request in {ControllerName} with ID: {Id}.", nameof(CrudController), id);

			var crud = await _repository.Get(id);
			if (crud == null)
				{
				_logger.LogError("Invalid DELETE attempt in {ControllerName}.", nameof(CrudController));
				return BadRequest("Submitted data is invalid");
				}

			_repository.Remove(crud);
			await _unitOfWork.Save();

			_logger.LogInformation("Deleted Crud item with ID {CrudId} in {ControllerName}.", id, nameof(CrudController));

			return NoContent();
			}

		#endregion
		}
	}

using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.UIL.Controllers;

public class CrudController : GenericControllers<Crud, CrudDto>
{
	private readonly ILogger<CrudController> _logger;

	public CrudController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CrudController> logger) : base(unitOfWork,
		mapper)
	{
		_logger = logger;
	}

	[HttpPost]
	public override IActionResult Post([FromBody] CrudDto requestDto)
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
}
using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

public class DebtController : GenericControllers<Debt, DebtDto>
{
	private readonly ILogger<DebtController> _logger;

	public DebtController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DebtController> logger) : base(
		unitOfWork, mapper)
	{
		_logger = logger;
	}

	#region POST|Create - Used to create a new resource.

	[HttpPost]
	public override IActionResult Post([FromBody] DebtDto requestDto)
	{
		_logger.LogInformation($"will look for Entity with of name {nameof(requestDto)} and see if we get it.");

		if (!ModelState.IsValid)
		{
			_logger.LogError($"Invalid POST attempt in {nameof(requestDto)}");
			return BadRequest(ModelState);
		}

		var mappedResult = Mapper.Map<Debt>(requestDto);

		Repository.Insert(mappedResult);
		UnitOfWork.SaveChanges();

		_logger.LogCritical($"The ID of Entity with of name {nameof(requestDto)} is {mappedResult.Id} .");

		return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
	}

	#endregion
}
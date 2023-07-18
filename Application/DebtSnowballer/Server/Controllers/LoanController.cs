using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

public class LoanController : GenericControllers<Loan, LoanDto>
{
	private readonly ILogger<LoanController> _logger;

	public LoanController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LoanController> logger) : base(
		unitOfWork, mapper)
	{
		_logger = logger;
	}

	#region POST|Create - Used to create a new resource.

	[HttpPost]
	public override IActionResult Post([FromBody] LoanDto requestDto)
	{
		_logger.LogInformation($"will look for Entity with of name {nameof(requestDto)} and see if we get it.");

		if (!ModelState.IsValid)
		{
			_logger.LogError($"Invalid POST attempt in {nameof(requestDto)}");
			return BadRequest(ModelState);
		}

		var mappedResult = Mapper.Map<Loan>(requestDto);

		Repository.Insert(mappedResult);
		UnitOfWork.SaveChanges();

		_logger.LogCritical($"The ID of Entity with of name {nameof(requestDto)} is {mappedResult.Id} .");

		return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
	}

	#endregion
}
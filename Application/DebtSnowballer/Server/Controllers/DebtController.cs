using System.Text.Json;
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
        _logger.LogInformation($"Received POST request in {nameof(DebtController)} with DTO: {JsonSerializer.Serialize(requestDto)}");

        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(DebtController)}. ModelState: {JsonSerializer.Serialize(ModelState)}");
            return BadRequest(ModelState);
        }

        var mappedResult = Mapper.Map<Debt>(requestDto);

        Repository.Insert(mappedResult);
        UnitOfWork.SaveChanges();

        _logger.LogInformation($"Successfully created entity in {nameof(DebtController)}. Entity ID: {mappedResult.Id}");

        return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
    }



    #endregion
    }
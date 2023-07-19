using System.Text.Json;
using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using DebtSnowballer.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        _logger.LogInformation(
            $"Received POST request in {nameof(DebtController)} with DTO: {JsonSerializer.Serialize(requestDto)}");

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

        var mappedResult = Mapper.Map<Debt>(requestDto);

        Repository.Insert(mappedResult);
        UnitOfWork.SaveChanges();

        _logger.LogInformation(
            $"Successfully created entity in {nameof(DebtController)}. Entity ID: {mappedResult.Id}");

        return CreatedAtAction(nameof(Get), new { id = mappedResult.Id }, mappedResult);
    }
	#endregion

	#region GET|Read - Used to retrieve a resource or a collection of resources.

	[HttpGet("{Auth0SUD}")]
	public IActionResult GetDebtbySUD(string Auth0SUD)
    {
        var debts = Repository.GetAll().Where(debt => debt.Auth0UserId == Auth0SUD);
        var mappedDebts = Mapper.Map<IList<DebtDto>>(debts);
        return Ok(mappedDebts);
    }
    #endregion

    }
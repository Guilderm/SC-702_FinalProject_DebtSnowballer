using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class DebtManagement
{
	private readonly ILogger<DebtManagement> _logger;
	private readonly IMapper _mapper;
	private readonly IGenericRepository<LoanDetail> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DebtManagement(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DebtManagement> logger)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = unitOfWork.GetRepository<LoanDetail>();
		_logger = logger;
	}

	public async Task<LoanDetailDto> CreateNewLoan(LoanDetailDto loanDetailDto, string auth0UserId)
	{
		_logger.LogInformation($"Executing {nameof(CreateNewLoan)} for user {auth0UserId}.");
		LoanDetail debt = _mapper.Map<LoanDetail>(loanDetailDto);
		debt.Auth0UserId = auth0UserId;
		await _repository.Insert(debt);
		await _unitOfWork.Save();
		_logger.LogInformation($"Successfully created loan with ID {debt.Id}.");
		return _mapper.Map<LoanDetailDto>(debt);
	}

	public async Task<LoanDetailDto> UpdateDebt(int id, LoanDetailDto loanDetailDto, string auth0UserId)
	{
		_logger.LogInformation($"Executing {nameof(UpdateDebt)} for debt ID {id} and user {auth0UserId}.");
		LoanDetail existingDebt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		_mapper.Map(loanDetailDto, existingDebt);
		_repository.Update(existingDebt);
		await _unitOfWork.Save();
		_logger.LogInformation($"Successfully updated debt with ID {id}.");
		return _mapper.Map<LoanDetailDto>(existingDebt);
	}

	public async Task DeleteDebt(int id, string auth0UserId)
	{
		_logger.LogInformation($"Executing {nameof(DeleteDebt)} for debt ID {id} and user {auth0UserId}.");
		LoanDetail debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		await _repository.Delete(debt.Id);
		await _unitOfWork.Save();
		_logger.LogInformation($"Successfully deleted debt with ID {id}.");
	}

	public async Task<LoanDetailDto> GetDebt(int id, string auth0UserId)
	{
		_logger.LogInformation($"Executing {nameof(GetDebt)} for debt ID {id} and user {auth0UserId}.");
		LoanDetail debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		return _mapper.Map<LoanDetailDto>(debt);
	}

	public async Task<IList<LoanDetailDto>> GetAllDebtsInQuoteCurrency(string auth0UserId)
	{
		_logger.LogInformation($"Executing {nameof(GetAllDebtsInQuoteCurrency)} for user {auth0UserId}.");
		IList<LoanDetail> debts = await _repository.GetAll(d => d.Auth0UserId == auth0UserId);
		return _mapper.Map<IList<LoanDetailDto>>(debts);
	}
}
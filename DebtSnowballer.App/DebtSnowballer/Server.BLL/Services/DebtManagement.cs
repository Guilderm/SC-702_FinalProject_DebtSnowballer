using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class DebtManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<Debt> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DebtManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = _unitOfWork.DebtRepository;
	}

	public async Task<DebtDto> CreateDebt(DebtDto debtDto)
	{
		Debt debt = _mapper.Map<Debt>(debtDto);
		await _repository.Insert(debt);
		await _unitOfWork.Save();
		return _mapper.Map<DebtDto>(debt);
	}

	public async Task<DebtDto> UpdateDebt(int id, DebtDto debtDto)
	{
		Debt existingDebt = await _repository.Get(d => d.Id == id);
		_mapper.Map(debtDto, existingDebt);
		_repository.Update(existingDebt);
		await _unitOfWork.Save();
		return _mapper.Map<DebtDto>(existingDebt);
	}

	public async Task DeleteDebt(int id)
	{
		await _repository.Delete(id);
		await _unitOfWork.Save();
	}

	public async Task<IList<DebtDto>> GetAllDebts(string auth0UserId)
	{
		IList<Debt> debts = await _repository.GetAll(d => d.Auth0UserId == auth0UserId);
		return _mapper.Map<IList<DebtDto>>(debts);
	}

	public async Task<DebtDto> GetDebt(int id, string auth0UserId)
	{
		Debt debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		return _mapper.Map<DebtDto>(debt);
	}
}
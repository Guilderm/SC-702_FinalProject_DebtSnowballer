using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class DebtManagement
{
	private readonly CurrencyService _currencyService;
	private readonly IMapper _mapper;
	private readonly IGenericRepository<Debt> _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserProfileManagement _userProfileManagement;

	public DebtManagement(IUnitOfWork unitOfWork, IMapper mapper, UserProfileManagement userProfileManagement,
		CurrencyService currencyService)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_userProfileManagement = userProfileManagement;
		_currencyService = currencyService;
		_repository = unitOfWork.GetRepository<Debt>();
	}

	public async Task<DebtDto> CreateDebt(DebtDto debtDto, string auth0UserId)
	{
		Debt debt = _mapper.Map<Debt>(debtDto);
		debt.Auth0UserId = auth0UserId;
		await _repository.Insert(debt);
		await _unitOfWork.Save();
		return _mapper.Map<DebtDto>(debt);
	}

	public async Task<DebtDto> UpdateDebt(int id, DebtDto debtDto, string auth0UserId)
	{
		Debt existingDebt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		_mapper.Map(debtDto, existingDebt);
		_repository.Update(existingDebt);
		await _unitOfWork.Save();
		return _mapper.Map<DebtDto>(existingDebt);
	}

	public async Task DeleteDebt(int id, string auth0UserId)
	{
		Debt debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		await _repository.Delete(debt.Id);
		await _unitOfWork.Save();
	}

	public async Task<IList<DebtDto>> GetAllDebtsInQuoteCurrency(string auth0UserId)
	{
		IList<Debt> debts = await _repository.GetAll(d => d.Auth0UserId == auth0UserId);
		return _mapper.Map<IList<DebtDto>>(debts);
	}

	public async Task<DebtDto> GetDebt(int id, string auth0UserId)
	{
		Debt debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
		return _mapper.Map<DebtDto>(debt);
	}

	public async Task<IList<DebtDto>> GetAllDebtsInBaseCurrency(string auth0UserId)
	{
		UserProfile userProfile = await _userProfileManagement.GetUserProfileModel(auth0UserId);

		IList<Debt> debtsInQuoteCurrency = await _repository.GetAll(d => d.Auth0UserId == auth0UserId);
		IList<DebtDto> debtsInBaseCurrency = _mapper.Map<IList<DebtDto>>(debtsInQuoteCurrency);

		foreach (DebtDto debtDto in debtsInBaseCurrency)
			if (debtDto.CurrencyCode != userProfile.BaseCurrency)
			{
				decimal exchangeRate =
					await _currencyService.GetExchangeRate(debtDto.CurrencyCode, userProfile.BaseCurrency);
				debtDto.RemainingPrincipal *= exchangeRate;
				debtDto.MonthlyPayment *= exchangeRate;
				debtDto.CurrencyCode = userProfile.BaseCurrency;
			}

		return debtsInBaseCurrency;
	}
}
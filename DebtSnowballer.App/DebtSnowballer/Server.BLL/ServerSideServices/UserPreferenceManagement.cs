using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class UserPreferenceManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<UserPreference> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UserPreferenceManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = _unitOfWork.GetRepository<UserPreference>();
	}


	public async Task<decimal> GetDebtPlanMonthlyPayment(string auth0UserId)
	{
		UserPreference userPreferenceModel = await GetUserPreferenceModel(auth0UserId);
		return userPreferenceModel.DebtPlanMonthlyPayment;
	}

	public async Task<bool> UpdateBaseCurrency(string baseCurrency, string auth0UserId)
	{
		UserPreference userPreferenceModel = await GetUserPreferenceModel(auth0UserId);

		if (userPreferenceModel.BaseCurrency == baseCurrency)
			return false;

		userPreferenceModel.BaseCurrency = baseCurrency;
		_repository.Update(userPreferenceModel);
		await _unitOfWork.Save();

		return true;
	}

	public async Task<UserPreferenceDto> PatchSelectedStrategy(string auth0UserId, int strategyTypeId)
	{
		UserPreference userPreferenceModel = await GetUserPreferenceModel(auth0UserId);
		if (userPreferenceModel == null) return null;

		userPreferenceModel.SelectedStrategy = strategyTypeId;
		_repository.Update(userPreferenceModel);
		await _unitOfWork.Save();

		UserPreferenceDto userPreferenceDto = _mapper.Map<UserPreferenceDto>(userPreferenceModel);

		return userPreferenceDto;
	}

	internal async Task<UserPreference> GetUserPreferenceModel(string auth0UserId)
	{
		UserPreference userPreferenceModel = await _repository.Get(u => u.Auth0UserId == auth0UserId);

		return userPreferenceModel == null
			? throw new Exception($"User profile not found for Auth0UserId: {auth0UserId}")
			: userPreferenceModel;
	}
}
using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class UserProfileManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<UserProfile> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UserProfileManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = _unitOfWork.GetRepository<UserProfile>();
	}

	public async Task<UserProfileDto> GetValidateUserProfile(UserProfileDto rawUserProfile, string auth0UserId)
	{
		bool changesMade = false;
		UserProfile userProfileModel = await GetUserProfileModel(auth0UserId);

		if (userProfileModel == null)
		{
			userProfileModel = _mapper.Map(rawUserProfile, userProfileModel);
			await _repository.Insert(userProfileModel);
			changesMade = true;
		}
		else if (rawUserProfile.LastUpdated > userProfileModel.LastUpdated)
		{
			_mapper.Map(rawUserProfile, userProfileModel);
			_repository.Update(userProfileModel);
			changesMade = true;
		}

		if (changesMade)
		{
			await _unitOfWork.Save();
			userProfileModel = await GetUserProfileModel(rawUserProfile.Auth0UserId);
		}

		UserProfileDto userProfileDto = _mapper.Map<UserProfileDto>(userProfileModel);
		return userProfileDto;
	}

	public async Task<decimal> GetDebtPlanMonthlyPayment(string auth0UserId)
	{
		UserProfile userProfileModel = await GetUserProfileModel(auth0UserId);
		return userProfileModel.DebtPlanMonthlyPayment;
	}

	public async Task<bool> UpdateBaseCurrency(string baseCurrency, string auth0UserId)
	{
		UserProfile userProfileModel = await GetUserProfileModel(auth0UserId);

		if (userProfileModel.BaseCurrency == baseCurrency)
			return false;

		userProfileModel.BaseCurrency = baseCurrency;
		_repository.Update(userProfileModel);
		await _unitOfWork.Save();

		return true;
	}

	public async Task<UserProfileDto> PatchSelectedStrategy(string auth0UserId, int strategyTypeId)
	{
		UserProfile userProfileModel = await GetUserProfileModel(auth0UserId);
		if (userProfileModel == null) return null;

		userProfileModel.SelectedStrategy = strategyTypeId;
		_repository.Update(userProfileModel);
		await _unitOfWork.Save();

		UserProfileDto userProfileDto = _mapper.Map<UserProfileDto>(userProfileModel);

		return userProfileDto;
	}

	internal async Task<UserProfile> GetUserProfileModel(string auth0UserId)
	{
		UserProfile userProfileModel = await _repository.Get(u => u.Auth0UserId == auth0UserId);

		return userProfileModel == null
			? throw new Exception($"User profile not found for Auth0UserId: {auth0UserId}")
			: userProfileModel;
	}
}
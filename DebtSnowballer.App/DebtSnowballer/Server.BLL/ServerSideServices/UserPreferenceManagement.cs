using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class UserPreferenceManagement
{
	private readonly ILogger<UserPreferenceManagement> _logger;
	private readonly IMapper _mapper;
	private readonly IGenericRepository<UserPreference> _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserProfileManagement _userProfileManagement;

	public UserPreferenceManagement(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserPreferenceManagement> logger,
		UserProfileManagement userProfileManagement)
	{
		_mapper = mapper;
		_unitOfWork = unitOfWork;
		_repository = unitOfWork.GetRepository<UserPreference>();
		_logger = logger;
		_userProfileManagement = userProfileManagement;
	}

	public async Task<UserPreferenceDto> GetUserPreference(string auth0UserId)
	{
		_logger.LogInformation($"Getting user preference for user: {auth0UserId}");
		UserPreference userPreference = await _repository.Get(u => u.Auth0UserId == auth0UserId);
		if (userPreference == null)
		{
			_logger.LogWarning($"User preference not found for user: {auth0UserId}");
			return new UserPreferenceDto(); // Return an empty DTO
		}

		_logger.LogInformation($"User preference retrieved for user: {auth0UserId}");
		return _mapper.Map<UserPreferenceDto>(userPreference);
	}

	public async Task<UserPreferenceDto> UpdateUserPreference(UserPreferenceDto newUserPreferenceDto,
		string auth0UserId)
	{
		_logger.LogInformation($"Updating user preference for user: {auth0UserId}");
		UserProfile userProfile = await _unitOfWork.GetRepository<UserProfile>().Get(u => u.Auth0UserId == auth0UserId);
		if (userProfile == null)
		{
			_logger.LogWarning($"User profile not found for user: {auth0UserId}. Creating a new profile.");
			await _userProfileManagement.CreateUserProfile(auth0UserId);
		}

		UserPreference oldUserPreference = await _repository.Get(u => u.Auth0UserId == auth0UserId);
		if (oldUserPreference == null)
		{
			_logger.LogWarning($"User preference not found for user: {auth0UserId}");
			oldUserPreference = new UserPreference { Auth0UserId = auth0UserId };
			// Set any other required properties here
			_repository.Insert(oldUserPreference);
		}


		_mapper.Map(newUserPreferenceDto, oldUserPreference);
		_repository.Update(oldUserPreference);
		await _unitOfWork.Save();
		_logger.LogInformation($"User preference updated for user: {auth0UserId}");
		return _mapper.Map<UserPreferenceDto>(oldUserPreference);
	}
}
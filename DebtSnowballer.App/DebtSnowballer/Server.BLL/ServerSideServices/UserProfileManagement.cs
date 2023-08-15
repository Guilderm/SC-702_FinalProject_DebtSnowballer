using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class UserProfileManagement
{
	private readonly ILogger<UserProfileManagement> _logger;
	private readonly IMapper _mapper;
	private readonly IGenericRepository<UserProfile> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UserProfileManagement(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserProfileManagement> logger)
	{
		_mapper = mapper;
		_unitOfWork = unitOfWork;
		_repository = unitOfWork.GetRepository<UserProfile>();
		_logger = logger;
	}

	public async Task<UserProfileDto> CreateUserProfile(string auth0UserId)
	{
		_logger.LogInformation($"Creating user profile for user: {auth0UserId}");
		UserProfileDto newUserProfileDto = new()
		{
			Auth0UserId = auth0UserId
		};
		UserProfile newUserProfile = _mapper.Map<UserProfile>(newUserProfileDto);
		await _repository.Insert(newUserProfile);
		await _unitOfWork.Save();
		_logger.LogInformation($"User profile created for user: {auth0UserId}");
		return _mapper.Map<UserProfileDto>(newUserProfile);
	}

	public async Task<UserProfileDto> GetUserProfile(string auth0UserId)
	{
		_logger.LogInformation($"Getting user profile for user: {auth0UserId}");
		UserProfile userProfile = await _repository.Get(u => u.Auth0UserId == auth0UserId);
		if (userProfile == null)
		{
			_logger.LogWarning($"User profile not found for user: {auth0UserId}. Creating a new profile.");
			return await CreateUserProfile(auth0UserId); // Call CreateUserProfile if the profile is not found
		}

		_logger.LogInformation($"User profile retrieved for user: {auth0UserId}");
		return _mapper.Map<UserProfileDto>(userProfile);
	}
}
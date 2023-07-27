using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class UserProfileManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<UserProfile> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UserProfileManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = _unitOfWork.UserProfileRepository;
	}

	public async Task<UserProfileDto> GetUserProfile(string auth0UserId)
	{
		UserProfile userProfile = await _repository.Get(u => u.Auth0UserId == auth0UserId);
		return _mapper.Map<UserProfileDto>(userProfile);
	}

	public async Task UpdateUserProfile(UserProfileDto userProfileDto, string auth0UserId)
	{
		UserProfile existingUserProfile = await _repository.Get(u => u.Auth0UserId == auth0UserId);
		_mapper.Map(userProfileDto, existingUserProfile);
		_repository.Update(existingUserProfile);
		await _unitOfWork.Save();
	}
}
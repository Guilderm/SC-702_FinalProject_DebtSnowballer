using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class SnowflakeManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<Snowflake> _repository;
	private readonly IUnitOfWork _unitOfWork;

	public SnowflakeManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_repository = _unitOfWork.GetRepository<Snowflake>();
	}

	public async Task<SnowflakeDto> CreateSnowflake(SnowflakeDto snowflakeDto, string auth0UserId)
	{
		Snowflake snowflake = _mapper.Map<Snowflake>(snowflakeDto);
		snowflake.Auth0UserId = auth0UserId;
		await _repository.Insert(snowflake);
		await _unitOfWork.Save();
		return _mapper.Map<SnowflakeDto>(snowflake);
	}

	public async Task<SnowflakeDto> UpdateSnowflake(int id, SnowflakeDto snowflakeDto, string auth0UserId)
	{
		Snowflake existingSnowflake = await _repository.Get(s => s.Id == id && s.Auth0UserId == auth0UserId);
		_mapper.Map(snowflakeDto, existingSnowflake);
		_repository.Update(existingSnowflake);
		await _unitOfWork.Save();
		return _mapper.Map<SnowflakeDto>(existingSnowflake);
	}

	public async Task DeleteSnowflake(int id, string auth0UserId)
	{
		Snowflake snowflake = await _repository.Get(s => s.Id == id && s.Auth0UserId == auth0UserId);
		await _repository.Delete(snowflake.Id);
		await _unitOfWork.Save();
	}

	public async Task<IList<SnowflakeDto>> GetAllSnowflakes(string auth0UserId)
	{
		IList<Snowflake> snowflakes = await _repository.GetAll(s => s.Auth0UserId == auth0UserId);
		return _mapper.Map<IList<SnowflakeDto>>(snowflakes);
	}

	public async Task<SnowflakeDto> GetSnowflake(int id, string auth0UserId)
	{
		Snowflake snowflake = await _repository.Get(s => s.Id == id && s.Auth0UserId == auth0UserId);
		return _mapper.Map<SnowflakeDto>(snowflake);
	}
}
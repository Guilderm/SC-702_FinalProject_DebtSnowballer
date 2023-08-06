using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Models;

namespace Server.BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<Debt, DebtDto>().ReverseMap();
		CreateMap<UserProfile, UserProfileDto>().ReverseMap();
		CreateMap<Snowflake, SnowflakeDto>().ReverseMap();
		CreateMap<ExchangeRate, ExchangeRateDto>().ReverseMap();
		CreateMap<StrategyType, StrategyTypeDto>().ReverseMap();
	}
}
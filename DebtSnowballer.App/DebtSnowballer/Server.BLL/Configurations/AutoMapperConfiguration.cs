using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Models;

namespace Server.BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<LoanDetail, LoanDetailDto>().ReverseMap();
		CreateMap<UserProfile, UserProfileDto>().ReverseMap();
		CreateMap<UserPreference, UserPreferenceDto>().ReverseMap();
		CreateMap<PlannedSnowflake, PlannedSnowflakeDto>().ReverseMap();
		CreateMap<ExchangeRate, ExchangeRateDto>().ReverseMap();
		CreateMap<DebtPayDownMethod, DebtPayDownMethodDto>().ReverseMap();
	}
}
using AutoMapper;
using DebtSnowballer.Shared.DTOs;

namespace BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<LoanDto, LoanDto>().ReverseMap();
	}
}
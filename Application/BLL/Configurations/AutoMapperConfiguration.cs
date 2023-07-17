using AutoMapper;
using DAL.Models;

namespace BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<LoanDto, LoanDto>().ReverseMap();
	}
}
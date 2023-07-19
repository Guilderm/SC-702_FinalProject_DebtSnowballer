using AutoMapper;
using DAL.Models;
using DebtSnowballer.Shared.DTOs;

namespace BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<Debt, DebtDto>().ReverseMap();
		CreateMap<Crud, CrudDto>().ReverseMap();
	}
}
using AutoMapper;
using DAL.Models;
using DebtSnowballer.Shared.DTOs;

namespace BLL.Configurations;

public class AutoMapperConfiguration : Profile
{
	public AutoMapperConfiguration()
	{
		CreateMap<Loan, LoanDto>().ReverseMap();
        CreateMap<Crud, CrudDto>().ReverseMap();
    }
}
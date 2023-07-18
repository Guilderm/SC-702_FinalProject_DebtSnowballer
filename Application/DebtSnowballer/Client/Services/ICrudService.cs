using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface ICrudService
{
	Task<List<CrudDto>> GetCrudItems();
	Task<CrudDto> GetCrudItem(int id);
	Task<CrudDto> AddCrudItem(CrudDto crudDto);
	Task<CrudDto> UpdateCrudItem(CrudDto crudDto);
	Task DeleteCrudItem(int id);
}
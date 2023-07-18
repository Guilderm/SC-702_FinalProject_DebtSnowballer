using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class CrudService : ICrudService
{
	private readonly HttpClient _httpClient;

	public CrudService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<CrudDto>> GetCrudItems()
	{
		return await _httpClient.GetFromJsonAsync<List<CrudDto>>("api/Crud");
	}

	public async Task<CrudDto> GetCrudItem(int id)
	{
		return await _httpClient.GetFromJsonAsync<CrudDto>($"api/Crud/{id}");
	}

	public async Task<CrudDto> AddCrudItem(CrudDto crudDto)
	{
		var response = await _httpClient.PostAsJsonAsync("api/Crud", crudDto);
		return await response.Content.ReadFromJsonAsync<CrudDto>();
	}

	public async Task<CrudDto> UpdateCrudItem(CrudDto crudDto)
	{
		var response = await _httpClient.PutAsJsonAsync($"api/Crud/{crudDto.Id}", crudDto);
		return await response.Content.ReadFromJsonAsync<CrudDto>();
	}

	public async Task DeleteCrudItem(int id)
	{
		await _httpClient.DeleteAsync($"api/Crud/{id}");
	}
}
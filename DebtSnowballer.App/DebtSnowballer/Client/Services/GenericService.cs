using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
	protected readonly string _APIURL;
	protected readonly HttpClient _httpClient;

	public GenericService(HttpClient httpClient, string APIURL)
	{
		_httpClient = httpClient;
		_APIURL = APIURL;
	}

	public async Task<List<T>> GetItems()
	{
		return await _httpClient.GetFromJsonAsync<List<T>>(_APIURL);
	}

	public async Task<T> GetItem(int id)
	{
		return await _httpClient.GetFromJsonAsync<T>($"{_APIURL}/{id}");
	}

	public async Task<T> AddItem(T item)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_APIURL, item);
		return await response.Content.ReadFromJsonAsync<T>();
	}

	public async Task<T> UpdateItem(T item)
	{
		HttpResponseMessage response =
			await _httpClient.PutAsJsonAsync($"{_APIURL}/{((CrudDto)(object)item).Id}", item);
		return await response.Content.ReadFromJsonAsync<T>();
	}

	public async Task DeleteItem(int id)
	{
		await _httpClient.DeleteAsync($"{_APIURL}/{id}");
	}
}
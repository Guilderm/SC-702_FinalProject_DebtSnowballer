using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly string Apiurl;
    protected readonly HttpClient HttpClient;

    public GenericService(HttpClient httpClient, string apiurl)
    {
        HttpClient = httpClient;
        Apiurl = apiurl;
    }

    public async Task<List<T>> GetItems()
    {
        return await HttpClient.GetFromJsonAsync<List<T>>(Apiurl);
    }

    public async Task<T> GetItem(int id)
    {
        return await HttpClient.GetFromJsonAsync<T>($"{Apiurl}/{id}");
    }

    public async Task<T> AddItem(T item)
    {
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(Apiurl, item);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T> UpdateItem(T item)
    {
        HttpResponseMessage response =
            await HttpClient.PutAsJsonAsync($"{Apiurl}/{((CrudDto)(object)item).Id}", item);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteItem(int id)
    {
        await HttpClient.DeleteAsync($"{Apiurl}/{id}");
    }
}
using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public class SnowflakeService : ISnowflakeService
{
    private readonly string _backendUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SnowflakeService> _logger;

    public SnowflakeService(HttpClient httpClient, ILogger<SnowflakeService> logger)
    {
        _httpClient = httpClient;
        _backendUrl = _httpClient.BaseAddress + "api/Snowflake";
        _logger = logger;
    }

    public async Task DeleteSnowflake(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error deleting snowflake: {response.ReasonPhrase}");
            throw new Exception($"Error deleting snowflake: {response.ReasonPhrase}");
        }
    }

    public async Task<IList<PlannedSnowflakeDto>> GetAllSnowflakes()
    {
        IList<PlannedSnowflakeDto> snowflakes =
            await _httpClient.GetFromJsonAsync<IList<PlannedSnowflakeDto>>($"{_backendUrl}");
        _logger.LogInformation($"Retrieved {snowflakes.Count} snowflakes.");
        return snowflakes;
    }

    public async Task<PlannedSnowflakeDto> GetSnowflakeById(int id)
    {
        PlannedSnowflakeDto plannedSnowflake =
            await _httpClient.GetFromJsonAsync<PlannedSnowflakeDto>($"{_backendUrl}/{id}");
        _logger.LogInformation($"Retrieved snowflake with ID {id}.");
        return plannedSnowflake;
    }

    public async Task<PlannedSnowflakeDto> AddSnowflake(PlannedSnowflakeDto plannedSnowflakeDto)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, plannedSnowflakeDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error posting snowflake: {response.ReasonPhrase}");
            throw new Exception($"Error posting snowflake: {response.ReasonPhrase}");
        }

        PlannedSnowflakeDto result = await response.Content.ReadFromJsonAsync<PlannedSnowflakeDto>();
        _logger.LogInformation($"Added new snowflake with ID {result.Id}.");
        return result;
    }

    public async Task<PlannedSnowflakeDto> UpdateSnowflake(PlannedSnowflakeDto plannedSnowflakeDto)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync($"{_backendUrl}/{plannedSnowflakeDto.Id}", plannedSnowflakeDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error updating snowflake: {response.ReasonPhrase}");
            throw new Exception($"Error updating snowflake: {response.ReasonPhrase}");
        }

        PlannedSnowflakeDto result = await response.Content.ReadFromJsonAsync<PlannedSnowflakeDto>();
        _logger.LogInformation($"Updated snowflake with ID {result.Id}.");
        return result;
    }
}
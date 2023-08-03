using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace DebtSnowballer.Client.Services
	{
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

		public async Task<IList<SnowflakeDto>> GetAllSnowflakes()
			{
			IList<SnowflakeDto> snowflakes = await _httpClient.GetFromJsonAsync<IList<SnowflakeDto>>($"{_backendUrl}");
			_logger.LogInformation($"Retrieved {snowflakes.Count} snowflakes.");
			return snowflakes;
			}

		public async Task<SnowflakeDto> GetSnowflakeById(int id)
			{
			SnowflakeDto snowflake = await _httpClient.GetFromJsonAsync<SnowflakeDto>($"{_backendUrl}/{id}");
			_logger.LogInformation($"Retrieved snowflake with ID {id}.");
			return snowflake;
			}

		public async Task<SnowflakeDto> AddSnowflake(SnowflakeDto snowflakeDto)
			{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_backendUrl, snowflakeDto);
			if (!response.IsSuccessStatusCode)
				{
				_logger.LogError($"Error posting snowflake: {response.ReasonPhrase}");
				throw new Exception($"Error posting snowflake: {response.ReasonPhrase}");
				}
			SnowflakeDto result = await response.Content.ReadFromJsonAsync<SnowflakeDto>();
			_logger.LogInformation($"Added new snowflake with ID {result.Id}.");
			return result;
			}

		public async Task<SnowflakeDto> UpdateSnowflake(SnowflakeDto snowflakeDto)
			{
			HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_backendUrl}/{snowflakeDto.Id}", snowflakeDto);
			if (!response.IsSuccessStatusCode)
				{
				_logger.LogError($"Error updating snowflake: {response.ReasonPhrase}");
				throw new Exception($"Error updating snowflake: {response.ReasonPhrase}");
				}
			SnowflakeDto result = await response.Content.ReadFromJsonAsync<SnowflakeDto>();
			_logger.LogInformation($"Updated snowflake with ID {result.Id}.");
			return result;
			}
		}
	}

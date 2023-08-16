using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface ISnowflakeService
{
	Task<SnowflakeDto> CreateSnowflake(SnowflakeDto snowflakeDto);
	Task DeleteSnowflake(int id);
	Task<List<SnowflakeDto>> GetAllSnowflakes();
	Task<SnowflakeDto> GetSnowflakeById(int id);
	Task<SnowflakeDto> UpdateSnowflake(SnowflakeDto snowflakeDto);
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.Services;

public interface ISnowflakeService
{
	Task<SnowflakeDto> AddSnowflake(SnowflakeDto snowflakeDto);
	Task DeleteSnowflake(int id);
	Task<IList<SnowflakeDto>> GetAllSnowflakes();
	Task<SnowflakeDto> GetSnowflakeById(int id);
	Task<SnowflakeDto> UpdateSnowflake(SnowflakeDto snowflakeDto);
}
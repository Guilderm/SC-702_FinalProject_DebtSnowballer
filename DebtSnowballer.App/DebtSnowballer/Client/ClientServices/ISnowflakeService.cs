using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientServices;

public interface ISnowflakeService
{
	Task<SnowflakeDto> AddSnowflake(SnowflakeDto snowflakeDto);
	Task DeleteSnowflake(int id);
	Task<IList<SnowflakeDto>> GetAllSnowflakes();
	Task<SnowflakeDto> GetSnowflakeById(int id);
	Task<SnowflakeDto> UpdateSnowflake(SnowflakeDto snowflakeDto);
}
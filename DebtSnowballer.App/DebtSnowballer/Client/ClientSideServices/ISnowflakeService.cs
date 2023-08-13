using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface ISnowflakeService
{
    Task<PlannedSnowflakeDto> AddSnowflake(PlannedSnowflakeDto plannedSnowflakeDto);
    Task DeleteSnowflake(int id);
    Task<IList<PlannedSnowflakeDto>> GetAllSnowflakes();
    Task<PlannedSnowflakeDto> GetSnowflakeById(int id);
    Task<PlannedSnowflakeDto> UpdateSnowflake(PlannedSnowflakeDto plannedSnowflakeDto);
}
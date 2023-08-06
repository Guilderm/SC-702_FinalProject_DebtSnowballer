using DebtSnowballer.Shared.DTOs;

namespace Server.BLL.ServerServices;

public interface IMultiPurposeService
{
	Task<List<StrategyTypeDto>> GetAllStrategyTypesAsync();
}
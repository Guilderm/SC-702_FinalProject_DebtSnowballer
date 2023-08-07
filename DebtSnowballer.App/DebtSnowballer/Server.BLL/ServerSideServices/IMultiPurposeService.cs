using DebtSnowballer.Shared.DTOs;

namespace Server.BLL.ServerSideServices;

public interface IMultiPurposeService
{
	Task<List<StrategyTypeDto>> GetAllStrategyTypesAsync();
}
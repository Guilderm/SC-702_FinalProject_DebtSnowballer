using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientServices;

public interface IMultiPurposeService
{
	Task<IList<StrategyTypeDto>> GetAllStrategyTypes();
}
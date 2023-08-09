using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices;

public interface IMultiPurposeService
{
	Task<IList<DebtPayDownMethodDto>> GetAllStrategyTypes();
}
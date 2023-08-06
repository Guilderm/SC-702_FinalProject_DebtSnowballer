using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerServices;

public class MultiPurposeManagement
{
	private readonly IMapper _mapper;
	private readonly IGenericRepository<StrategyType> _strategyTypeRepository;
	private readonly IUnitOfWork _unitOfWork;

	public MultiPurposeManagement(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_strategyTypeRepository = _unitOfWork.GetRepository<StrategyType>();
	}

	public async Task<IList<StrategyTypeDto>> GetAllStrategyTypes()
	{
		IList<StrategyType> strategyTypes = await _strategyTypeRepository.GetAll();
		return _mapper.Map<IList<StrategyTypeDto>>(strategyTypes);
	}
}
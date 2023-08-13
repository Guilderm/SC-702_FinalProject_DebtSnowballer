using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class MultiPurposeManagement
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<DebtPayDownMethod> _strategyTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MultiPurposeManagement(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _strategyTypeRepository = _unitOfWork.GetRepository<DebtPayDownMethod>();
    }

    public async Task<IList<DebtPayDownMethodDto>> GetAllDebtPayDownMethods()
    {
        IList<DebtPayDownMethod> payDownMethods = await _strategyTypeRepository.GetAll();
        return _mapper.Map<IList<DebtPayDownMethodDto>>(payDownMethods);
    }
}
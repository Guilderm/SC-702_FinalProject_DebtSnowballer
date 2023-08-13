using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.ServerSideServices;

public class DebtManagement
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<LoanDetail> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DebtManagement(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repository = unitOfWork.GetRepository<LoanDetail>();
    }

    public async Task<LoanDetailDto> CreateDebt(LoanDetailDto loanDetailDto, string auth0UserId)
    {
        LoanDetail debt = _mapper.Map<LoanDetail>(loanDetailDto);
        debt.Auth0UserId = auth0UserId;
        await _repository.Insert(debt);
        await _unitOfWork.Save();
        return _mapper.Map<LoanDetailDto>(debt);
    }

    public async Task<LoanDetailDto> UpdateDebt(int id, LoanDetailDto loanDetailDto, string auth0UserId)
    {
        LoanDetail existingDebt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
        _mapper.Map(loanDetailDto, existingDebt);
        _repository.Update(existingDebt);
        await _unitOfWork.Save();
        return _mapper.Map<LoanDetailDto>(existingDebt);
    }

    public async Task DeleteDebt(int id, string auth0UserId)
    {
        LoanDetail debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
        await _repository.Delete(debt.Id);
        await _unitOfWork.Save();
    }

    public async Task<LoanDetailDto> GetDebt(int id, string auth0UserId)
    {
        LoanDetail debt = await _repository.Get(d => d.Id == id && d.Auth0UserId == auth0UserId);
        return _mapper.Map<LoanDetailDto>(debt);
    }

    public async Task<IList<LoanDetailDto>> GetAllDebtsInQuoteCurrency(string auth0UserId)
    {
        IList<LoanDetail> debts = await _repository.GetAll(d => d.Auth0UserId == auth0UserId);
        return _mapper.Map<IList<LoanDetailDto>>(debts);
    }
}
using AutoMapper;
using DebtSnowballer.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BLL.Services;

public class CrudManagement
{
    private readonly ILogger<CrudManagement> _logger;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Crud> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CrudManagement(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CrudManagement> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _repository = unitOfWork.CrudRepository;
    }

    public async Task<CrudDto> CreateCrud(CrudDto crudDto)
    {
        Crud crud = _mapper.Map<Crud>(crudDto);
        await _repository.Insert(crud);
        await _unitOfWork.Save();

        _logger.LogInformation("Created new Crud item with ID {CrudId}.", crud.Id);

        CrudDto createdCrudDto = _mapper.Map<CrudDto>(crud);
        return createdCrudDto;
    }

    public async Task<CrudDto> UpdateCrud(int id, CrudDto crudDto)
    {
        Crud crud = await _repository.Get(d => d.Id == id);
        _mapper.Map(crudDto, crud);
        _repository.Update(crud);
        await _unitOfWork.Save();

        _logger.LogInformation("Updated Crud item with ID {CrudId}.", id);

        CrudDto updatedCrudDto = _mapper.Map<CrudDto>(crud);
        return updatedCrudDto;
    }

    public async Task DeleteCrud(int id)
    {
        await _repository.Delete(id);
        await _unitOfWork.Save();

        _logger.LogInformation("Deleted Crud item with ID {CrudId}.", id);
    }

    public async Task<IList<CrudDto>> GetAllCruds()
    {
        IList<Crud> cruds = await _repository.GetAll();
        IList<CrudDto> results = _mapper.Map<IList<CrudDto>>(cruds);

        _logger.LogInformation("Returned {Count} Crud items.", results.Count);

        return results;
    }

    public async Task<CrudDto> GetCrud(int id)
    {
        Crud crud = await _repository.Get(d => d.Id == id);
        CrudDto result = _mapper.Map<CrudDto>(crud);

        _logger.LogInformation("Returned Crud item with ID {CrudId}.", id);

        return result;
    }
}
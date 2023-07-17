using AutoMapper;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenericControllers<TEntity, TModel> : ControllerBase
    where TEntity : class, new()
    where TModel : class, new()
{
    private readonly ILogger<GenericControllers<TEntity, TModel>> _logger;
    protected readonly IMapper Mapper;
    protected readonly IGenericRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;

    public GenericControllers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        UnitOfWork = unitOfWork;
        Repository = UnitOfWork.GetRepository<TEntity>();
        Mapper = mapper;
        _logger = new LoggerFactory().CreateLogger<GenericControllers<TEntity, TModel>>();
    }

    #region POST|Create - Used to create a new resource.

    [HttpPost]
    public virtual IActionResult Post([FromBody] TModel requestDto)
    {
        _logger.LogInformation($"Registration Attempt for {requestDto} ");

        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(requestDto)}");
            return BadRequest(ModelState);
        }

        TEntity? mappedResult = Mapper.Map<TEntity>(requestDto);

        Repository.Insert(mappedResult);
        UnitOfWork.SaveChanges();

        return Ok(mappedResult);
    }

    #endregion

    #region PUT|Update - Used to update an existing resource.

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] TModel requestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get the existing entity from the database
        TEntity existingEntity = Repository.Get(id);
        if (existingEntity == null)
        {
            // If the entity does not exist, return a 404 Not Found status
            return NotFound();
        }

        // Map the request DTO to the existing entity
        Mapper.Map(requestDto, existingEntity);

        // Update the entity in the database
        Repository.Update(existingEntity);
        UnitOfWork.SaveChanges();

        // Map the updated entity back to a DTO to return in the response
        TModel updatedDto = Mapper.Map<TModel>(existingEntity);

        return Ok(updatedDto);
    }

    #endregion

    #region PATCH|Update - Used to partially update an existing resource.

    [HttpPatch]
    public IActionResult Patch()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region DELETE|Delete - Used to delete a resource.

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        TEntity dbResult = Repository.Get(id);
        Repository.Remove(dbResult);
        UnitOfWork.SaveChanges();

        return NoContent();
    }

    #endregion

    #region GET|Read - Used to retrieve a resource or a collection of resources.

    [HttpGet]
    public IActionResult Get()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        IEnumerable<TEntity> dbResult = Repository.GetAll();
        IList<TModel>? mappedResult = Mapper.Map<IList<TModel>>(dbResult);
        return Ok(mappedResult);
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        TEntity dbResult = Repository.Get(id);
        TModel? mappedResult = Mapper.Map<TModel>(dbResult);
        return Ok(mappedResult);
    }

    #endregion
}
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;

namespace Server.DAL.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	private readonly DbContext _DbContext;
	private readonly DbSet<TEntity> _dbSet;
	private readonly ILogger<GenericRepository<TEntity>> _logger;

	public GenericRepository(ILogger<GenericRepository<TEntity>> logger, DbContext dbContext)
	{
		_DbContext = dbContext;
		_dbSet = _DbContext.Set<TEntity>();
		_logger = logger;
	}

	public void Insert(TEntity entity)
	{
		_dbSet.Add(entity);
	}

	public void AddRange(IEnumerable<TEntity> entities)
	{
		_DbContext.Set<TEntity>().AddRange(entities);
	}

	public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
	{
		try
		{
			return _DbContext.Set<TEntity>().Where(predicate);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while finding entities with predicate {Predicate}", predicate);
			return Enumerable.Empty<TEntity>();
		}
	}

	public TEntity Get(int id)
	{
		_logger.LogInformation("Looking for entity of type {EntityType} with id {Id}", typeof(TEntity), id);
		var entity = _DbContext.Set<TEntity>().Find(id);

		if (entity == null)
			_logger.LogError("Entity of type {EntityType} with id {Id} not found", typeof(TEntity), id);

		return entity;
	}

	public IEnumerable<TEntity> GetAll()
	{
		try
		{
			return _DbContext.Set<TEntity>().ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while getting all entities of type {EntityType}", typeof(TEntity));
			return Enumerable.Empty<TEntity>();
		}
	}

	public bool Remove(TEntity entity)
	{
		try
		{
			_DbContext.Set<TEntity>().Attach(entity);
			_DbContext.Set<TEntity>().Remove(entity);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while removing entity of type {EntityType}", typeof(TEntity));
			return false;
		}
	}

	public void RemoveRange(IEnumerable<TEntity> entities)
	{
		_DbContext.Set<TEntity>().RemoveRange(entities);
	}

	public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
	{
		try
		{
			var entity = _DbContext.Set<TEntity>().SingleOrDefault(predicate);
			return entity ?? throw new InvalidOperationException($"Entity with id {predicate} not found.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving entity with predicate {Predicate}", predicate);
			throw;
		}
	}

	public bool Update(TEntity entity)
	{
		try
		{
			_DbContext.Entry(entity).State = EntityState.Modified;
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while updating entity of type {EntityType}", typeof(TEntity));
			return false;
		}
	}
}
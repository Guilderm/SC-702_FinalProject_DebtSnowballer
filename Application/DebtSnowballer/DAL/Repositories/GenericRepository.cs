using System.Linq.Expressions;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	private readonly DbContext _DbContext;
	private readonly DbSet<TEntity> _dbSet;
	private readonly ILogger<GenericRepository<TEntity>> _logger;

	public GenericRepository(ILogger<GenericRepository<TEntity>> logger, DbContext dbContex)
	{
		_DbContext = dbContex;
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
		catch (Exception)
		{
			return Enumerable.Empty<TEntity>();
		}
	}

	public TEntity Get(int id)
	{
		_logger.LogInformation($"will look for Entity with id {id}.");
		var entity = _DbContext.Set<TEntity>().Find(id);

		if (entity == null) _logger.LogError($"Entity of type {typeof(TEntity)}, with id {id} not found.");

		return entity;
	}

	public IEnumerable<TEntity> GetAll()
	{
		try
		{
			return _DbContext.Set<TEntity>().ToList();
		}
		catch (Exception)
		{
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
		catch (Exception)
		{
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
			throw new Exception($"Error retrieving entity with id {predicate}: {ex.Message}");
		}
	}

	public bool Update(TEntity entity)
	{
		try
		{
			_DbContext.Entry(entity).State = EntityState.Modified;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
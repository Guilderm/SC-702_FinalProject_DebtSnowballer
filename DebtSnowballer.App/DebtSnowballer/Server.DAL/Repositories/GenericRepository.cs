using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;

namespace Server.DAL.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	protected readonly DbContext DbContext;
	protected readonly DbSet<TEntity> DbSet;
	protected readonly ILogger<GenericRepository<TEntity>> Logger;

	public GenericRepository(ILogger<GenericRepository<TEntity>> logger, DbContext dbContext)
	{
		DbContext = dbContext;
		DbSet = DbContext.Set<TEntity>();
		Logger = logger;
	}

	public async Task Insert(TEntity entity)
	{
		Logger.LogInformation($"Inserting an entity of type {typeof(TEntity).Name} into the database.");
		await DbSet.AddAsync(entity);
		Logger.LogInformation($"Entity inserted: {entity}");
	}

	public async Task InsertRange(IEnumerable<TEntity> entities)
	{
		Logger.LogInformation($"Inserting entities of type {typeof(TEntity).Name} into the database.");
		await DbSet.AddRangeAsync(entities);
		Logger.LogInformation($"Entities inserted: {entities}");
	}

	public async Task<TEntity> Get(Expression<Func<TEntity, bool>> expression,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
	{
		Logger.LogInformation($"Getting an entity of type {typeof(TEntity).Name} from the database.");
		IQueryable<TEntity> query = DbSet;
		if (include != null) query = include(query);

		TEntity? entity = await query.AsNoTracking().FirstOrDefaultAsync(expression);
		Logger.LogInformation($"Entity retrieved: {entity}");
		return entity;
	}

	public async Task<IList<TEntity>> GetAll(Expression<Func<TEntity, bool>> expression = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
	{
		Logger.LogInformation($"Getting all entities of type {typeof(TEntity).Name} from the database.");
		IQueryable<TEntity> query = DbSet;

		if (expression != null) query = query.Where(expression);

		if (include != null) query = include(query);

		if (orderBy != null) query = orderBy(query);

		List<TEntity> entities = await query.AsNoTracking().ToListAsync();
		Logger.LogInformation($"Entities retrieved: {entities}");
		return entities;
	}

	public async Task Delete(int id)
	{
		Logger.LogInformation($"Deleting an entity of type {typeof(TEntity).Name} with id {id} from the database.");
		TEntity? entity = await DbSet.FindAsync(id);
		DbSet.Remove(entity);
		Logger.LogInformation($"Entity deleted: {entity}");
	}

	public void DeleteRange(IEnumerable<TEntity> entities)
	{
		Logger.LogInformation($"Deleting entities of type {typeof(TEntity).Name} from the database.");
		DbSet.RemoveRange(entities);
		Logger.LogInformation($"Entities deleted: {entities}");
	}

	public void Update(TEntity entity)
	{
		Logger.LogInformation($"Updating an entity of type {typeof(TEntity).Name} in the database.");
		DbSet.Attach(entity);
		DbContext.Entry(entity).State = EntityState.Modified;
		Logger.LogInformation($"Entity updated: {entity}");
	}

	public async Task Delete(Expression<Func<TEntity, bool>> predicate)
	{
		Logger.LogInformation($"Deleting entities of type {typeof(TEntity).Name} from the database.");
		var entities = await DbSet.Where(predicate).ToListAsync();
		DbSet.RemoveRange(entities);
		Logger.LogInformation($"Entities deleted: {entities}");
	}
}
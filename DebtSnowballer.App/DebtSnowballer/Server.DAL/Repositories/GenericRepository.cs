using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;

namespace Server.DAL.Repositories
	{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
		{
		protected readonly DbContext _DbContext;
		protected readonly DbSet<TEntity> _dbSet;
		protected readonly ILogger<GenericRepository<TEntity>> _logger;

		public GenericRepository(ILogger<GenericRepository<TEntity>> logger, DbContext dbContext)
			{
			_DbContext = dbContext;
			_dbSet = _DbContext.Set<TEntity>();
			_logger = logger;
			}

		public async Task Insert(TEntity entity)
			{
			_logger.LogInformation($"Inserting an entity of type {typeof(TEntity).Name} into the database.");
			await _dbSet.AddAsync(entity);
			_logger.LogInformation($"Entity inserted: {entity}");
			}

		public async Task InsertRange(IEnumerable<TEntity> entities)
			{
			_logger.LogInformation($"Inserting entities of type {typeof(TEntity).Name} into the database.");
			await _dbSet.AddRangeAsync(entities);
			_logger.LogInformation($"Entities inserted: {entities}");
			}

		public async Task<TEntity> Get(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
			{
			_logger.LogInformation($"Getting an entity of type {typeof(TEntity).Name} from the database.");
			IQueryable<TEntity> query = _dbSet;
			if (include != null)
				{
				query = include(query);
				}

			TEntity? entity = await query.AsNoTracking().FirstOrDefaultAsync(expression);
			_logger.LogInformation($"Entity retrieved: {entity}");
			return entity;
			}

		public async Task<IList<TEntity>> GetAll(Expression<Func<TEntity, bool>> expression = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
			{
			_logger.LogInformation($"Getting all entities of type {typeof(TEntity).Name} from the database.");
			IQueryable<TEntity> query = _dbSet;

			if (expression != null)
				{
				query = query.Where(expression);
				}

			if (include != null)
				{
				query = include(query);
				}

			if (orderBy != null)
				{
				query = orderBy(query);
				}

			List<TEntity> entities = await query.AsNoTracking().ToListAsync();
			_logger.LogInformation($"Entities retrieved: {entities}");
			return entities;
			}

		public async Task Delete(int id)
			{
			_logger.LogInformation($"Deleting an entity of type {typeof(TEntity).Name} with id {id} from the database.");
			TEntity? entity = await _dbSet.FindAsync(id);
			_dbSet.Remove(entity);
			_logger.LogInformation($"Entity deleted: {entity}");
			}

		public void DeleteRange(IEnumerable<TEntity> entities)
			{
			_logger.LogInformation($"Deleting entities of type {typeof(TEntity).Name} from the database.");
			_dbSet.RemoveRange(entities);
			_logger.LogInformation($"Entities deleted: {entities}");
			}

		public void Update(TEntity entity)
			{
			_logger.LogInformation($"Updating an entity of type {typeof(TEntity).Name} in the database.");
			_dbSet.Attach(entity);
			_DbContext.Entry(entity).State = EntityState.Modified;
			_logger.LogInformation($"Entity updated: {entity}");
			}
		}
	}

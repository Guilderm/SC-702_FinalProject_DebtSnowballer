using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Server.DAL.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
	Task Insert(TEntity entity);
	Task InsertRange(IEnumerable<TEntity> entities);

	Task<TEntity> Get(Expression<Func<TEntity, bool>> expression,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

	Task<IList<TEntity>> GetAll(Expression<Func<TEntity, bool>> expression = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

	Task Delete(int id);
	void DeleteRange(IEnumerable<TEntity> entities);
	void Update(TEntity entity);
}
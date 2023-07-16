using System.Linq.Expressions;

namespace DAL.Interfaces;

public interface IGenericRepository<TEntity>
{
	void Insert(TEntity entity);
	void AddRange(IEnumerable<TEntity> entities);
	IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
	TEntity Get(int id);
	IEnumerable<TEntity> GetAll();
	bool Remove(TEntity entity);
	void RemoveRange(IEnumerable<TEntity> entities);
	TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
	bool Update(TEntity entity);
}
﻿namespace DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

	void SaveChanges();
}
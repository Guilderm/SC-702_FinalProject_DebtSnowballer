using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL.Repositories
	{
	public class UnitOfWork : IUnitOfWork, IDisposable
		{
		private readonly DebtSnowballerContext _dbContext;
		private readonly ILogger<UnitOfWork> _logger;
		private readonly ILoggerFactory _loggerFactory;
		private bool _disposed;

		public UnitOfWork(ILogger<UnitOfWork> logger, DebtSnowballerContext dbContext, ILoggerFactory loggerFactory)
			{
			_loggerFactory = loggerFactory;
			_logger = logger;
			_dbContext = dbContext;
			_logger.LogInformation("Created a new {UnitOfWork} instance", nameof(UnitOfWork));
			}

		public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
			{
			return new GenericRepository<TEntity>(
				_loggerFactory.CreateLogger<GenericRepository<TEntity>>(), _dbContext);
			}

		public void SaveChanges()
			{
			try
				{
				var rowsAffected = _dbContext.SaveChanges();
				_logger.LogInformation("EF affected {RowsAffected} rows when saving changes", rowsAffected);
				}
			catch (DbUpdateException ex)
				{
				_logger.LogError(ex, "We got a {DbUpdateException}", nameof(DbUpdateException));
				}
			catch (Exception ex)
				{
				_logger.LogError(ex, "We got a {Exception}", nameof(Exception));
				}
			}

		public void Dispose()
			{
			Dispose(true);
			GC.SuppressFinalize(this);
			}

		protected virtual void Dispose(bool disposing)
			{
			if (!_disposed)
				if (disposing)
					{
					_dbContext.Dispose();
					_logger.LogInformation("Disposed the {UnitOfWork} instance", nameof(UnitOfWork));
					}

			_disposed = true;
			}
		}
	}
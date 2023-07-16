using DAL.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly NorthwindContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private bool _disposed;

    public UnitOfWork(ILogger<UnitOfWork> logger, NorthwindContext dbContext, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = logger;
        _dbContext = dbContext;
        _logger.LogInformation("Created a new Unit of Work instance");
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
            int rowsAffected = _dbContext.SaveChanges();
            _logger.LogInformation($"EF affected {rowsAffected} rows when saving changes.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "We got a DB Update Exception");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "We got a Exception");
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
        {
            if (disposing)
            {
                _dbContext.Dispose();
                _logger.LogInformation("Disposed the Unit of Work instance");
            }
        }

        _disposed = true;
    }
}
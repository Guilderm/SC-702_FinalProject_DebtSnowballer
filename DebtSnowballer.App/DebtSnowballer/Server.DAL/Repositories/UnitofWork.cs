using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly DebtSnowballerDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public UnitOfWork(DebtSnowballerDbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        return new GenericRepository<TEntity>(
            _loggerFactory.CreateLogger<GenericRepository<TEntity>>(), _context);
    }

    public async Task Save()
    {
        _logger.LogInformation("Saving changes to the {DbContextName}.", nameof(_context));

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved changes to the {DbContextName}.", nameof(_context));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "We got a DbUpdateException while saving changes to the {DbContextName}.",
                nameof(_context));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes to the {DbContextName}.", nameof(_context));
            throw;
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing the {DbContextName}.", nameof(_context));
        _context.Dispose();
    }
}
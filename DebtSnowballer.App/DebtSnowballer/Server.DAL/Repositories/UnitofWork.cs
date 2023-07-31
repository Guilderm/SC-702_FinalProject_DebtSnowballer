using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DebtSnowballerContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private IGenericRepository<Crud> _cruds;
    private IGenericRepository<Debt> _debts;
    private IGenericRepository<UserProfile> _userProfile;

    public UnitOfWork(DebtSnowballerContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public IGenericRepository<Crud> CrudRepository => _cruds ??=
        new GenericRepository<Crud>(_loggerFactory.CreateLogger<GenericRepository<Crud>>(), _context);

    public IGenericRepository<Debt> DebtRepository => _debts ??=
        new GenericRepository<Debt>(_loggerFactory.CreateLogger<GenericRepository<Debt>>(), _context);

    public IGenericRepository<UserProfile> UserProfileRepository => _userProfile ??=
        new GenericRepository<UserProfile>(_loggerFactory.CreateLogger<GenericRepository<UserProfile>>(), _context);

    public async Task Save()
    {
        _logger.LogInformation("Saving changes to the {DbContextName}.", nameof(_context));

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Changes saved to the database.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "We got a DbUpdateException");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes to the {DbContextName}.", nameof(_context));
            throw;
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing the DbContext.");
        _context.Dispose();
    }
}
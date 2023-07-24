using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DbContext _context;
		private readonly ILogger<UnitOfWork> _logger;
		private IGenericRepository<Debt> _debts;
		private IGenericRepository<Crud> CRUDs;
		// Add more fields here for other repositories as needed

		public UnitOfWork(DbContext context, ILogger<UnitOfWork> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IGenericRepository<Crud> CrudRepository => CRUDs ??= new GenericRepository<Crud>(_logger, _context);
		public IGenericRepository<Debt> DebtRepository => _debts ??= new GenericRepository<Debt>(_logger, _context);

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
}
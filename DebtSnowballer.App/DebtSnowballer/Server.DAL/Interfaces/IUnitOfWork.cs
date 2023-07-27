using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<UserProfile> UserProfileRepository { get; }
	IGenericRepository<Debt> DebtRepository { get; }

	IGenericRepository<Crud> CrudRepository { get; }

	// Add more properties here for other repositories as needed
	Task Save();
}
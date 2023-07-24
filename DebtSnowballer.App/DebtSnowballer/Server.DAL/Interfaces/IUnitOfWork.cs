using Server.DAL.Interfaces;
using System;
using System.Threading.Tasks;
using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<Debt> Debts { get; }

	// Add more properties here for other repositories as needed
	Task Save();
}
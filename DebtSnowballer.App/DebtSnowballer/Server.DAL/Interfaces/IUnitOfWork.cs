using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<UserProfile> UserProfileRepository { get; }
	IGenericRepository<Debt> DebtRepository { get; }
	IGenericRepository<ExchangeRate> ExchangeRateRepository { get; }
	IGenericRepository<Snowflake> SnowflakeRepository { get; }
	Task Save();
}
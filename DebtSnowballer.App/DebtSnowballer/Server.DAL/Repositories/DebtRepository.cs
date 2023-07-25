using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL.Repositories;

public class DebtRepository : GenericRepository<Debt>, IDebtRepository
{
	public DebtRepository(ILogger<GenericRepository<Debt>> logger, DbContext dbContext)
		: base(logger, dbContext)
	{
	}

	public Debt Get(int id, string auth0UserId)
	{
		_logger.LogInformation("Looking for Debt entity with id {Id} and Auth0UserId {Auth0UserId}", id, auth0UserId);
		Debt? entity = _DbContext.Set<Debt>().SingleOrDefault(e => e.Id == id && e.Auth0UserId == auth0UserId);

		if (entity == null)
			_logger.LogError("Debt entity with id {Id} and Auth0UserId {Auth0UserId} not found", id, auth0UserId);

		return entity;
	}

	public IEnumerable<Debt> GetAll(string auth0UserId)
	{
		try
		{
			return _DbContext.Set<Debt>().Where(e => e.Auth0UserId == auth0UserId).ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while getting all Debt entities for Auth0UserId {Auth0UserId}",
				auth0UserId);
			return Enumerable.Empty<Debt>();
		}
	}

	public bool Remove(int id, string auth0UserId)
	{
		try
		{
			Debt? entityToRemove =
				_DbContext.Set<Debt>().SingleOrDefault(e => e.Id == id && e.Auth0UserId == auth0UserId);
			_DbContext.Set<Debt>().Remove(entityToRemove);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while removing Debt entity for Auth0UserId {Auth0UserId}",
				auth0UserId);
			return false;
		}
	}

	public void Insert(Debt entity, string auth0UserId)
	{
		entity.Auth0UserId = auth0UserId;
		_dbSet.Add(entity);
	}

	public bool Update(Debt entity, string auth0UserId)
	{
		try
		{
			Debt? existingEntity = _DbContext.Set<Debt>()
				.SingleOrDefault(e => e.Id == entity.Id && e.Auth0UserId == auth0UserId);
			if (existingEntity == null)
			{
				_logger.LogError("Entity not found in {RepositoryName} with ID: {Id}", nameof(DebtRepository),
					entity.Id);
				return false;
			}

			_DbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while updating Debt entity for Auth0UserId {Auth0UserId}",
				auth0UserId);
			return false;
		}
	}
}
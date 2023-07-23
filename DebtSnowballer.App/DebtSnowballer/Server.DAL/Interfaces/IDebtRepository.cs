﻿using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IDebtRepository : IGenericRepository<Debt>
{
	Debt Get(int id, string auth0UserId);
	IEnumerable<Debt> GetAll(string auth0UserId);
	bool Remove(int id, string auth0UserId);
	void Insert(Debt entity, string auth0UserId);
	bool Update(Debt entity, string auth0UserId);
}
namespace Server.DAL.Models;

public class DebtStrategy
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; } = null!;
	public int UserId { get; set; }
	public int StrategyId { get; set; }

	public virtual StrategyType Strategy { get; set; } = null!;
	public virtual UserProfile User { get; set; } = null!;
}
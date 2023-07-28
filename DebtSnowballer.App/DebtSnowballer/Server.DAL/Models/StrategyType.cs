namespace Server.DAL.Models;

public class StrategyType
{
	public StrategyType()
	{
		DebtStrategies = new HashSet<DebtStrategy>();
	}

	public int Id { get; set; }
	public string Type { get; set; } = null!;

	public virtual ICollection<DebtStrategy> DebtStrategies { get; set; }
}
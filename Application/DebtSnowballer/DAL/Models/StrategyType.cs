namespace DAL.Models;

public class StrategyType
{
	public StrategyType()
	{
		PaymentStrategyPlans = new HashSet<PaymentStrategyPlan>();
	}

	public int Id { get; set; }
	public string Type { get; set; } = null!;
	public bool HasCustomStrategy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public virtual ICollection<PaymentStrategyPlan> PaymentStrategyPlans { get; set; }
}
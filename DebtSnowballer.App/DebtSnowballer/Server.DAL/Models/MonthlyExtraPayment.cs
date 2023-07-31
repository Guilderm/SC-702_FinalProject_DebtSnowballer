namespace Server.DAL.Models;

public class MonthlyExtraPayment
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public decimal Amount { get; set; }

	public virtual UserProfile User { get; set; } = null!;
}
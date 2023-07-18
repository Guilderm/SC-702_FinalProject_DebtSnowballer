namespace DAL.Models;

public class PaymentStrategyPlan
{
	public PaymentStrategyPlan()
	{
		DebtSnowflakes = new HashSet<DebtSnowflake>();
		LoanCardinalOrders = new HashSet<LoanCardinalOrder>();
		Loans = new HashSet<Loan>();
	}

	public int Id { get; set; }
	public int UserId { get; set; }
	public int StrategyTypeId { get; set; }
	public decimal GlobalMonthlyPayment { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public virtual StrategyType StrategyType { get; set; } = null!;
	public virtual User User { get; set; } = null!;
	public virtual ICollection<DebtSnowflake> DebtSnowflakes { get; set; }
	public virtual ICollection<LoanCardinalOrder> LoanCardinalOrders { get; set; }
	public virtual ICollection<Loan> Loans { get; set; }
}
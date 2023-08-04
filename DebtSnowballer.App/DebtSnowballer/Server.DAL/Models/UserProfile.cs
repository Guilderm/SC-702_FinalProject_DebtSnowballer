namespace Server.DAL.Models;

public class UserProfile
{
	public UserProfile()
	{
		Debts = new HashSet<Debt>();
		SessionLogs = new HashSet<SessionLog>();
		Snowflakes = new HashSet<Snowflake>();
	}

	public int Id { get; set; }
	public string Auth0UserId { get; set; } = null!;
	public string? GivenName { get; set; }
	public string? FamilyName { get; set; }
	public string? NickName { get; set; }
	public string? FullName { get; set; }
	public string? Email { get; set; }
	public string? Picture { get; set; }
	public string? Locale { get; set; }
	public int UserTypeId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdated { get; set; }
	public string BaseCurrency { get; set; } = null!;
	public decimal DebtPlanMonthlyPayment { get; set; }
	public int SelectedStrategy { get; set; }
	public decimal TotalAmountOwed { get; set; }
	public decimal AggregatedMonthlyPayment { get; set; }

	public virtual StrategyType SelectedStrategyNavigation { get; set; } = null!;
	public virtual ICollection<Debt> Debts { get; set; }
	public virtual ICollection<SessionLog> SessionLogs { get; set; }
	public virtual ICollection<Snowflake> Snowflakes { get; set; }
}
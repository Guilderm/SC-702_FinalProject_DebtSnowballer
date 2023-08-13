namespace Server.DAL.Models;

public class PlannedSnowflake
{
    public int Id { get; set; }
    public string Auth0UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int FrequencyInMonths { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartingAt { get; set; }
    public DateTime EndingAt { get; set; }
    public string CurrencyCode { get; set; } = null!;

    public virtual UserProfile Auth0User { get; set; } = null!;
    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;
}
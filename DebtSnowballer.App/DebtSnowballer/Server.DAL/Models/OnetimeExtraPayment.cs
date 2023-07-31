namespace Server.DAL.Models;

public class OnetimeExtraPayment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public virtual UserProfile User { get; set; } = null!;
}
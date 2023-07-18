namespace DAL.Models;

public class Currency
{
	public Currency()
	{
		Loans = new HashSet<Loan>();
	}

	public int Id { get; set; }
	public string FormalName { get; set; } = null!;
	public string ShortName { get; set; } = null!;
	public string Symbol { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public virtual ICollection<Loan> Loans { get; set; }
}
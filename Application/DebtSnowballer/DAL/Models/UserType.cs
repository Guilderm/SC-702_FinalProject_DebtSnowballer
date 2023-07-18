namespace DAL.Models;

public class UserType
{
	public UserType()
	{
		Users = new HashSet<User>();
	}

	public int Id { get; set; }
	public string Type { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public virtual ICollection<User> Users { get; set; }
}
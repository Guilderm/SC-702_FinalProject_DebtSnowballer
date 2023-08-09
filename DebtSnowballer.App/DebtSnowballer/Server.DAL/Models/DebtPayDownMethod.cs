namespace Server.DAL.Models;

public class DebtPayDownMethod
{
	public DebtPayDownMethod()
	{
		UserProfiles = new HashSet<UserProfile>();
	}

	public int Id { get; set; }
	public string Name { get; set; } = null!;

	public virtual ICollection<UserProfile> UserProfiles { get; set; }
}
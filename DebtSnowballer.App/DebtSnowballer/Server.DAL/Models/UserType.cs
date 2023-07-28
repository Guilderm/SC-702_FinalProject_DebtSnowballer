namespace Server.DAL.Models;

public class UserType
{
	public int Id { get; set; }
	public string Type { get; set; } = null!;
	public string Description { get; set; } = null!;
}
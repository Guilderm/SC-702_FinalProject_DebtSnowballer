namespace Server.DAL.Models;

public class UserRole
{
    public UserRole()
    {
        UserProfiles = new HashSet<UserProfile>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public virtual ICollection<UserProfile> UserProfiles { get; set; }
}
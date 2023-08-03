namespace Server.DAL.Models;

public class SessionLog
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; } = null!;
	public DateTime LogonTimeStamp { get; set; }
	public DateTime? LogoffTimeStamp { get; set; }
	public string OperatingSystem { get; set; } = null!;
	public string ClientSoftware { get; set; } = null!;
	public string RemoteIpAddress { get; set; } = null!;

	public virtual UserProfile Auth0User { get; set; } = null!;
}
﻿namespace Server.DAL.Models;

public class UserProfile
{
	public UserProfile()
	{
		LoanDetails = new HashSet<LoanDetail>();
		PlannedSnowflakes = new HashSet<PlannedSnowflake>();
		SessionLogs = new HashSet<SessionLog>();
		UserPreferences = new HashSet<UserPreference>();
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
	public int UserRoleId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdated { get; set; }

	public virtual UserRole UserRole { get; set; } = null!;
	public virtual ICollection<LoanDetail> LoanDetails { get; set; }
	public virtual ICollection<PlannedSnowflake> PlannedSnowflakes { get; set; }
	public virtual ICollection<SessionLog> SessionLogs { get; set; }
	public virtual ICollection<UserPreference> UserPreferences { get; set; }
}
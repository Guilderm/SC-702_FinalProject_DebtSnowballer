namespace DebtSnowballer.Shared.DTOs;

public class UserProfileDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string GivenName { get; set; }
	public string FamilyName { get; set; }
	public string NickName { get; set; }
	public string FullName { get; set; }
	public string Email { get; set; }
	public string Picture { get; set; }
	public string Locale { get; set; }
	public int UserRoleId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdated { get; set; }
	public string UserRoleName { get; set; }
	public int NumberOfLoans { get; set; }
	public int NumberOfPlannedSnowflakes { get; set; }
	public int NumberOfSessionLogs { get; set; }
}
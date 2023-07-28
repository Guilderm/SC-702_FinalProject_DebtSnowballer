namespace DebtSnowballer.Shared.DTOs;

public class UserProfileDto
{
	public int Id { get; set; }
	public string Auth0UserId { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Email { get; set; }
	public string BaseCurrency { get; set; }
	public int UserTypeId { get; set; }
	public DateTime CreatedAt { get; set; }
}
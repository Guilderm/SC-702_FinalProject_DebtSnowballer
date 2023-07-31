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
	public string BaseCurrency { get; set; }
	public int UserTypeId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdated { get; set; }

	// If you need to transfer the related entities as well, uncomment these lines
	// public List<DebtStrategyDto> DebtStrategies { get; set; }
	// public List<MonthlyExtraPaymentDto> MonthlyExtraPayments { get; set; }
	// public List<OnetimeExtraPaymentDto> OnetimeExtraPayments { get; set; }
	// public List<SessionLogDto> SessionLogs { get; set; }
}
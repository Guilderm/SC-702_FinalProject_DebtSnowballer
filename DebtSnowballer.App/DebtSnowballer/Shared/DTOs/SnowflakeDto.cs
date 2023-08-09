using System.ComponentModel.DataAnnotations;

namespace DebtSnowballer.Shared.DTOs;

public class SnowflakeDto
{
	public int Id { get; set; }

	public string Auth0UserId { get; set; }

	[Required]
	[StringLength(50, ErrorMessage = "Nick Name must be less than 50 characters.")]
	public string NickName { get; set; }

	[Required]
	[Range(1, 12, ErrorMessage = "Frequency In Months must be between 1 and 12.")]
	public int FrequencyInMonths { get; set; }

	[Required]
	[Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
	public decimal Amount { get; set; }

	[Required] public DateTime? StartingAt { get; set; }

	[Required]
	[CustomValidation(typeof(SnowflakeDto), "ValidateEndingAt")]
	public DateTime? EndingAt { get; set; }

	[Required]
	[StringLength(3, ErrorMessage = "Currency must be a valid ISO 4217 alpha code.")]
	public string CurrencyCode { get; set; }

	public static ValidationResult ValidateEndingAt(DateTime? endingAt, ValidationContext context)
	{
		SnowflakeDto instance = context.ObjectInstance as SnowflakeDto;
		if (instance == null || instance.StartingAt == null)
			return ValidationResult.Success;

		if (endingAt > instance.StartingAt.Value.AddYears(45))
			return new ValidationResult("EndingAt must be less than 45 years from StartingAt.");

		return ValidationResult.Success;
	}
}
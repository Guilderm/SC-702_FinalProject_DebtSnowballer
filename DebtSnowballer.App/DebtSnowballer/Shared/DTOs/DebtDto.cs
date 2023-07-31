using System.ComponentModel.DataAnnotations;

namespace DebtSnowballer.Shared.DTOs;

public class DebtDto
{
	public int Id { get; set; }

	public string Auth0UserId { get; set; }

	[Required]
	[StringLength(50, ErrorMessage = "Loan Nick Name must be less than 50 characters.")]
	public string LoanNickName { get; set; }

	[Required]
	[Range(0, double.MaxValue, ErrorMessage = "Remaining Principal must be a positive number.")]
	public decimal RemainingPrincipal { get; set; }

	[Required]
	[Range(0, 90, ErrorMessage = "Interest Rate must be less than 90%.")]
	public decimal InterestRate { get; set; }

	[Range(0, double.MaxValue, ErrorMessage = "Fees must be a positive number.")]
	public decimal Fees { get; set; }

	[Required]
	[Range(0, double.MaxValue, ErrorMessage = "Monthly Payment must be a positive number.")]
	public decimal MonthlyPayment { get; set; }

	[Required]
	[Range(0, 541, ErrorMessage = "Remaining Term must be less than 45 years (540 months)")]
	public int RemainingTermInMonths { get; set; }

	[StringLength(3, ErrorMessage = "Currency must be a valid ISO 4217 alpha code.")]
	public string CurrencyCode { get; set; }

	[Range(0, 25)] public int CardinalOrder { get; set; }
}
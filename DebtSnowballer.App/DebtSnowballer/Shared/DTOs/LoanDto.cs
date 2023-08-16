using System.ComponentModel.DataAnnotations;

namespace DebtSnowballer.Shared.DTOs;

public class LoanDto
{
    public int Id { get; set; }

    public string Auth0UserId { get; set; }

    [Required] [StringLength(50)] public string Name { get; set; }

    [Required] [Range(0.01, 1000000000)] public decimal RemainingPrincipal { get; set; }

    [Required] [Range(0.01, 1000000000)] public decimal BankFees { get; set; }

    [Required] [Range(0.01, 1000000000)] public decimal ContractedMonthlyPayment { get; set; }

    [Required] [Range(0.0001, 1.0)] public decimal AnnualInterestRate { get; set; }

    [Required(ErrorMessage = "Remaining Term has to be between 2 moths and 45 years.")]
    [Range(2, 540)]
    public int RemainingTermInMonths { get; set; }

    [Required(ErrorMessage = "Currency must be a valid ISO 4217 alpha code.")]
    [StringLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    public int CardinalOrder { get; set; } = 1;

    [Required]
    [CustomValidation(typeof(LoanDto), "ValidateStartDate")]
    public DateTime StartDate { get; set; } =
        new(DateTime.Now.Year, DateTime.Now.Month, 1); // Default to the first day of the current month

    public static ValidationResult ValidateStartDate(DateTime startDate, ValidationContext context)
    {
        if (startDate < DateTime.Today)
            return new ValidationResult(
                "Loan cannot start in the past, please enter the data for the loan as it stands today.");

        return ValidationResult.Success;
    }
}
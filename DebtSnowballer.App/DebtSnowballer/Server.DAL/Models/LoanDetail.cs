namespace Server.DAL.Models;

public class LoanDetail
{
    public int Id { get; set; }
    public string Auth0UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal RemainingPrincipal { get; set; }
    public decimal BankFees { get; set; }
    public decimal ContractedMonthlyPayment { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int RemainingTermInMonths { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public int CardinalOrder { get; set; }
    public DateTime StartDate { get; set; }

    public virtual UserProfile Auth0User { get; set; } = null!;
    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;
}
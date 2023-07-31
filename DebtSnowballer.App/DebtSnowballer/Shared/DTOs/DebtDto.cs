namespace DebtSnowballer.Shared.DTOs;

public class DebtDto
{
    public int Id { get; set; }
    public string Auth0UserId { get; set; }
    public string LoanNickName { get; set; }
    public decimal RemainingPrincipal { get; set; }
    public decimal InterestRate { get; set; }
    public decimal Fees { get; set; }
    public decimal MonthlyPayment { get; set; }
    public int RemainingTerm { get; set; }
    public string CurrencyCode { get; set; }
    public int CardinalOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
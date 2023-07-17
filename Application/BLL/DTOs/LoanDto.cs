using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class LoanDto
{

    public int Id { get; set; }
    public string LoanNickName { get; set; } = null!;
    public int PaymentStrategy { get; set; }
    public decimal Principal { get; set; }
    public decimal InterestRate { get; set; }
    public decimal Fees { get; set; }
    public decimal MonthlyPayment { get; set; }
    public int RemainingTerm { get; set; }
    public int Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
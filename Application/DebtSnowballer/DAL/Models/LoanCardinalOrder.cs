using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class LoanCardinalOrder
{
    public int Id { get; set; }
    public int PaymentStrategy { get; set; }
    public int LoanId { get; set; }
    public int CardinalOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Loan Loan { get; set; } = null!;
    public virtual PaymentStrategyPlan PaymentStrategyNavigation { get; set; } = null!;
}
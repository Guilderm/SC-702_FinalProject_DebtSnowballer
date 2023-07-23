using System;
using System.Collections.Generic;

namespace DebtSnowballer.Server.Models
{
    public partial class Debt
    {
        public int Id { get; set; }
        public string Auth0UserId { get; set; }
        public string LoanNickName { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Fees { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int RemainingTerm { get; set; }
        public int CurrencyId { get; set; }
        public int CardinalOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Currency Currency { get; set; }
    }
}

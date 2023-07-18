using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class DebtSnowflake
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int PaymentStrategy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual PaymentStrategyPlan PaymentStrategyNavigation { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class MonthlyExtraPayment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }

        public virtual AppUser User { get; set; } = null!;
    }
}

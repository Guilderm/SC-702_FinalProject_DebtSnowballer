using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class DebtStrategy
    {
        public int Id { get; set; }
        public string Auth0UserId { get; set; } = null!;
        public int UserId { get; set; }
        public int StrategyId { get; set; }

        public virtual StrategyType Strategy { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
    }
}

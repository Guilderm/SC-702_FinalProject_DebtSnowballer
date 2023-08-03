using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class StrategyType
    {
        public StrategyType()
        {
            DebtStrategies = new HashSet<DebtStrategy>();
        }

        public int Id { get; set; }
        public string Type { get; set; } = null!;

        public virtual ICollection<DebtStrategy> DebtStrategies { get; set; }
    }
}

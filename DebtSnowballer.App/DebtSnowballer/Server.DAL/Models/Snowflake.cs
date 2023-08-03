using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class Snowflake
    {
        public int Id { get; set; }
        public string Auth0UserId { get; set; } = null!;
        public int Frequency { get; set; }
        public decimal Amount { get; set; }

        public virtual UserProfile Auth0User { get; set; } = null!;
    }
}

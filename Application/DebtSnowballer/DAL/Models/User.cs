using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class User
    {
        public User()
        {
            PaymentStrategyPlans = new HashSet<PaymentStrategyPlan>();
            SessionLogs = new HashSet<SessionLog>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int UserTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual UserType UserType { get; set; } = null!;
        public virtual ICollection<PaymentStrategyPlan> PaymentStrategyPlans { get; set; }
        public virtual ICollection<SessionLog> SessionLogs { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DebtSnowballer.Server.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            MonthlyExtraPayments = new HashSet<MonthlyExtraPayment>();
            OnetimeExtraPayments = new HashSet<OnetimeExtraPayment>();
            SessionLogs = new HashSet<SessionLog>();
        }

        public int Id { get; set; }
        public string Auth0UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<MonthlyExtraPayment> MonthlyExtraPayments { get; set; }
        public virtual ICollection<OnetimeExtraPayment> OnetimeExtraPayments { get; set; }
        public virtual ICollection<SessionLog> SessionLogs { get; set; }
    }
}

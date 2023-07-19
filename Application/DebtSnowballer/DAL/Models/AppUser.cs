﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            Loans = new HashSet<Loan>();
            MonthlyExtraPayments = new HashSet<MonthlyExtraPayment>();
            OnetimeExtraPayments = new HashSet<OnetimeExtraPayment>();
            SessionLogs = new HashSet<SessionLog>();
        }

        public int Id { get; set; }
        public string Auth0UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int UserTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<MonthlyExtraPayment> MonthlyExtraPayments { get; set; }
        public virtual ICollection<OnetimeExtraPayment> OnetimeExtraPayments { get; set; }
        public virtual ICollection<SessionLog> SessionLogs { get; set; }
    }
}

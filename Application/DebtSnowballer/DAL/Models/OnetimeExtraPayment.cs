﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class OnetimeExtraPayment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public virtual AppUser User { get; set; } = null!;
    }
}

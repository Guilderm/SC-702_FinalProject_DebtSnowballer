﻿using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class Debt
    {
        public int Id { get; set; }
        public string Auth0UserId { get; set; } = null!;
        public string LoanNickName { get; set; } = null!;
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Fees { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int RemainingTerm { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public int CardinalOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

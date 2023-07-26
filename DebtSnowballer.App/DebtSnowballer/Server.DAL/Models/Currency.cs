﻿using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class Currency
    {
        public Currency()
        {
            Debts = new HashSet<Debt>();
        }

        public int Id { get; set; }
        public string FormalName { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public string Symbol { get; set; } = null!;

        public virtual ICollection<Debt> Debts { get; set; }
    }
}

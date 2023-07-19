using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Currency
    {
        public Currency()
        {
            Loans = new HashSet<Loan>();
        }

        public int Id { get; set; }
        public string FormalName { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public string Symbol { get; set; } = null!;

        public virtual ICollection<Loan> Loans { get; set; }
    }
}

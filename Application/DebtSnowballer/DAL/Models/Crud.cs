using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Crud
    {
        public int Id { get; set; }
        public string LoanName { get; set; } = null!;
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int TermMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

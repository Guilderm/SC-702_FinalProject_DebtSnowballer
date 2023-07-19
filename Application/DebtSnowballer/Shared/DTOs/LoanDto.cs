using System;
using System.ComponentModel.DataAnnotations;

namespace DebtSnowballer.Shared.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }



        [Required]
        public string LoanName { get; set; }

       
        public decimal Principal { get; set; }

      
        public decimal InterestRate { get; set; }

      
        public int TermMonths { get; set; }

      
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
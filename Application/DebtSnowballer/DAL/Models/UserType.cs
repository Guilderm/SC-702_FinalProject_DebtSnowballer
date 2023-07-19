using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class UserType
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
    }
}

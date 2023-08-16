﻿using System;
using System.Collections.Generic;

namespace Server.DAL.Models
{
    public partial class DebtPayDownMethod
    {
        public DebtPayDownMethod()
        {
            UserPreferences = new HashSet<UserPreference>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}

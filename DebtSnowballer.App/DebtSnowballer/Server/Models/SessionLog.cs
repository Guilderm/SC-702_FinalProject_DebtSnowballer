using System;
using System.Collections.Generic;

namespace DebtSnowballer.Server.Models
{
    public partial class SessionLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LogonTimeStamp { get; set; }
        public DateTime? LogoffTimeStamp { get; set; }
        public string OperatingSystem { get; set; }
        public string ClientSoftware { get; set; }
        public string RemoteIpAddress { get; set; }

        public virtual AppUser User { get; set; }
    }
}

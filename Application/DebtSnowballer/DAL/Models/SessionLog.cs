using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class SessionLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime LogonTimeStamp { get; set; }
    public DateTime LogoffTimeStamp { get; set; }
    public string OperatingSystem { get; set; } = null!;
    public string ClientSoftware { get; set; } = null!;
    public string RemoteIpAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
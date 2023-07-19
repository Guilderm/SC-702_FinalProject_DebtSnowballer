using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DebtSnowballer.Shared.DTOs;
public class LogMessage
{
	public string Category { get; set; }
	public LogLevel LogLevel { get; set; }
	public int EventId { get; set; }
	public string State { get; set; }
	public string Exception { get; set; }
	public string Message { get; set; }
}

﻿namespace Server.DAL.Models;

public class ExchangeRate
{
	public int Id { get; set; }
	public string BaseCurrency { get; set; } = null!;
	public string QuoteCurrency { get; set; } = null!;
	public decimal ConversionRate { get; set; }
	public DateTime NextUpdateTime { get; set; }
}
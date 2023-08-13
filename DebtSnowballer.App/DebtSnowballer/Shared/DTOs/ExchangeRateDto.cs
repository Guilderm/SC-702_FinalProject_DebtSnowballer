namespace DebtSnowballer.Shared.DTOs;

public class ExchangeRateDto
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; }
    public string QuoteCurrency { get; set; }
    public decimal ConversionRate { get; set; }
    public DateTime NextUpdateTime { get; set; }
}
namespace DebtSnowballer.Shared.Currency;

public static class Currencies
{
	public static List<CurrencyInfo> All { get; } = new()
	{
		new()
		{
			Name = "United States dollar",
			AlphaCode = "USD",
			NumericCode = 840,
			Symbol = "$",
			Precision = 2
		},
		new()
		{
			Name = "Euro",
			AlphaCode = "EUR",
			NumericCode = 978,
			Symbol = "€",
			Precision = 2
		},
		new()
		{
			Name = "Japanese yen",
			AlphaCode = "JPY",
			NumericCode = 392,
			Symbol = "¥",
			Precision = 0
		},
		new()
		{
			Name = "Pound sterling",
			AlphaCode = "GBP",
			NumericCode = 826,
			Symbol = "£",
			Precision = 2
		},
		new()
		{
			Name = "Australian dollar",
			AlphaCode = "AUD",
			NumericCode = 36,
			Symbol = "$",
			Precision = 2
		},
		new()
		{
			Name = "Canadian dollar",
			AlphaCode = "CAD",
			NumericCode = 124,
			Symbol = "$",
			Precision = 2
		},
		new()
		{
			Name = "Swiss franc",
			AlphaCode = "CHF",
			NumericCode = 756,
			Symbol = "Fr",
			Precision = 2
		},
		new()
		{
			Name = "Renminbi (Chinese yuan)",
			AlphaCode = "CNY",
			NumericCode = 156,
			Symbol = "¥",
			Precision = 2
		},
		new()
		{
			Name = "Indian rupee",
			AlphaCode = "INR",
			NumericCode = 356,
			Symbol = "₹",
			Precision = 2
		},
		new()
		{
			Name = "Costa Rican colón",
			AlphaCode = "CRC",
			NumericCode = 188,
			Symbol = "₡",
			Precision = 2
		}
	};
}
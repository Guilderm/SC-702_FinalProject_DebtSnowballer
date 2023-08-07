namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class DebtPlanCalculator
{
	private readonly AmortizationCalculator _amortizationCalculator;
	private readonly DebtSortingStrategies _debtSortingStrategies;

	public DebtPlanCalculator(AmortizationCalculator amortizationCalculator,
		DebtSortingStrategies debtSortingStrategies)
	{
		_amortizationCalculator = amortizationCalculator;
		_debtSortingStrategies = debtSortingStrategies;
	}

	public PaymentPlanDetails CalculateStrategy(List<Debt> debts, decimal debtPlanMonthlyPayment, string strategy)
	{
		switch (strategy)
		{
			case "DebtSnowball":
				_debtSortingStrategies.SortByBalance(debts);
				break;
			case "DebtAvalanche":
				_debtSortingStrategies.SortByInterestRate(debts);
				break;
			default:
				throw new ArgumentException($"Invalid strategy: {strategy}");
		}

		decimal totalMinimumPayments = debts.Sum(d => d.MinimumPayment);
		decimal extraPayment = debtPlanMonthlyPayment - totalMinimumPayments;
		if (debtPlanMonthlyPayment < totalMinimumPayments) extraPayment = 0;


		PaymentPlanDetails debtPlan = new()
		{
			AmortizationSchedule = new Dictionary<Debt, List<PaymentPeriodDetail>>()
		};

		foreach (Debt loan in debts)
		{
			List<PaymentPeriodDetail> amortizationSchedule = _amortizationCalculator.CalculateAmortizationSchedule(
				loan.Balance, loan.AnnualInterestRate, loan.TermInMonths, loan.MonthlyBankFee, DateTime.Now,
				debtPlanMonthlyPayment + extraPayment);

			if (amortizationSchedule.Count > 0)
			{
				debtPlan.AmortizationSchedule.Add(loan, amortizationSchedule);

				PaymentPeriodDetail lastPaymentPeriod = amortizationSchedule[^1];

				debtPlan.TotalInterestPaid += lastPaymentPeriod.AccumulatedInterest;
				debtPlan.TotalBankFeesPaid += lastPaymentPeriod.AccumulatedBankFees;

				if (lastPaymentPeriod.PaymentPeriodDate > debtPlan.DebtFreeDate)
					debtPlan.DebtFreeDate = lastPaymentPeriod.PaymentPeriodDate;

				// Update the extra payment for the next debt
				extraPayment += debtPlanMonthlyPayment - lastPaymentPeriod.AmountAmortized;
			}
		}

		return debtPlan;
	}
}
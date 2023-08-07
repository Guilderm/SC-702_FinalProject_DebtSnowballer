namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class StrictDebtSnowball : IDebtPayoffStrategy
{
	private readonly AmortizationCalculator _amortizationCalculator;

	public StrictDebtSnowball(AmortizationCalculator amortizationCalculator)
	{
		_amortizationCalculator = amortizationCalculator;
	}

	public PaymentPlanDetails CalculateStrategy(List<Debt> debts)
	{
		// Sort the debts by balance from smallest to largest
		debts.Sort((a, b) => a.Balance.CompareTo(b.Balance));

		PaymentPlanDetails strictDebtSnowballPlan = new()
		{
			AmortizationSchedule = new Dictionary<Debt, List<PaymentPeriodDetail>>()
		};

		foreach (Debt loan in debts)
		{
			List<PaymentPeriodDetail> amortizationSchedule = _amortizationCalculator.CalculateAmortizationSchedule(
				loan.Balance, loan.AnnualInterestRate, loan.TermInMonths, loan.MonthlyBankFee, DateTime.Now);

			if (amortizationSchedule.Count > 0)
			{
				strictDebtSnowballPlan.AmortizationSchedule.Add(loan, amortizationSchedule);

				PaymentPeriodDetail lastPaymentPeriod = amortizationSchedule[^1];

				strictDebtSnowballPlan.TotalInterestPaid += lastPaymentPeriod.AccumulatedInterest;
				strictDebtSnowballPlan.TotalBankFeesPaid += lastPaymentPeriod.AccumulatedBankFees;

				if (lastPaymentPeriod.PaymentPeriodDate > strictDebtSnowballPlan.DebtFreeDate)
					strictDebtSnowballPlan.DebtFreeDate = lastPaymentPeriod.PaymentPeriodDate;
			}
		}

		return strictDebtSnowballPlan;
	}
}
using DebtSnowballer.Shared.DTOs;

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

	public PaymentPlanDetails CalculateStrategy(List<DebtDto> debts, decimal debtPlanMonthlyPayment, string strategy)
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

		decimal totalMinimumPayments = debts.Sum(d => d.MonthlyPayment);
		decimal extraPayment = debtPlanMonthlyPayment - totalMinimumPayments;
		if (debtPlanMonthlyPayment < totalMinimumPayments)
			extraPayment = 0;

		PaymentPlanDetails debtPlan = new()
		{
			AmortizationSchedule = new Dictionary<DebtDto, List<PaymentPeriodDetail>>()
		};

		foreach (DebtDto debt in debts)
		{
			List<PaymentPeriodDetail> amortizationSchedule =
				_amortizationCalculator.CalculateAmortizationSchedule(debt, DateTime.Now, extraPayment);

			if (amortizationSchedule.Count > 0)
			{
				debtPlan.AmortizationSchedule.Add(debt, amortizationSchedule);

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
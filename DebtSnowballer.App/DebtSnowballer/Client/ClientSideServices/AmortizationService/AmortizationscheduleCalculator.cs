﻿using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationscheduleCalculator
{
	private readonly DebtSortingStrategies _debtSortingStrategies;
	private readonly MonthlyPaymentCalculator _monthlyPaymentCalculator;

	public AmortizationscheduleCalculator(MonthlyPaymentCalculator monthlyPaymentCalculator,
		DebtSortingStrategies debtSortingStrategies)
	{
		_monthlyPaymentCalculator = monthlyPaymentCalculator;
		_debtSortingStrategies = debtSortingStrategies;
	}

	public Amortizationschedule CalculateStrategy(List<DebtDto> debts, decimal debtPlanMonthlyPayment, string strategy)
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

		Amortizationschedule debtPlan = new()
		{
			AmortizationSchedule = new Dictionary<DebtDto, List<MonthlyPayment>>()
		};

		foreach (DebtDto debt in debts)
		{
			List<MonthlyPayment> amortizationSchedule =
				_monthlyPaymentCalculator.CalculateAmortizationSchedule(debt, DateTime.Now, extraPayment);

			if (amortizationSchedule.Count > 0)
			{
				debtPlan.AmortizationSchedule.Add(debt, amortizationSchedule);

				MonthlyPayment lastMonthlyPaymentPeriod = amortizationSchedule[^1];

				debtPlan.TotalInterestPaid += lastMonthlyPaymentPeriod.AccumulatedInterest;
				debtPlan.TotalBankFeesPaid += lastMonthlyPaymentPeriod.AccumulatedBankFees;

				if (lastMonthlyPaymentPeriod.PaymentPeriodDate > debtPlan.DebtFreeDate)
					debtPlan.DebtFreeDate = lastMonthlyPaymentPeriod.PaymentPeriodDate;

				// Update the extra payment for the next debt
				extraPayment += debtPlanMonthlyPayment - lastMonthlyPaymentPeriod.AmountAmortized;
			}
		}

		return debtPlan;
	}
}
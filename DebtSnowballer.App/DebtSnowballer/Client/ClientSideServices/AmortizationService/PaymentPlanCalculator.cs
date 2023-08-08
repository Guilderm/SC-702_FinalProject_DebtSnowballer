﻿using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class PaymentPlanCalculator
{
	private readonly AmortizationScheduleCalculator _scheduleCalculator;

	public PaymentPlanCalculator(AmortizationScheduleCalculator amortizationScheduleCalculator)
	{
		_scheduleCalculator = amortizationScheduleCalculator;
	}

	public PaymentPlanDetail CalculatePaymentPlans(List<DebtDto> debts)
	{
		PaymentPlanDetail paymentPlanDetail = new();

		List<DebtDto> unsortedDebtsForBaseline = debts;
		List<DebtDto> sortedDebtsForSnowball = debts.OrderByDescending(d => d.RemainingPrincipal).ToList();
		List<DebtDto> sortedDebtsForAvalanche = debts.OrderByDescending(d => d.AnnualInterestRate).ToList();
		List<DebtDto> sortedDebtsForCustom = debts.OrderByDescending(d => d.CardinalOrder).ToList();

		paymentPlanDetail.PaymentPlans["Baseline"] =
			_scheduleCalculator.CalculateAmortizationSchedule(unsortedDebtsForBaseline);
		paymentPlanDetail.PaymentPlans["Snowball"] =
			_scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForSnowball);
		paymentPlanDetail.PaymentPlans["Avalanche"] =
			_scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForAvalanche);
		paymentPlanDetail.PaymentPlans["Custom"] =
			_scheduleCalculator.CalculateAmortizationSchedule(sortedDebtsForCustom);

		return paymentPlanDetail;
	}
}
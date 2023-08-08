using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationScheduleCalculator
{
	private readonly List<DebtDto> _debts;

	public AmortizationScheduleCalculator(List<DebtDto> debts)
	{
		_debts = debts;
	}

	public List<AmortizationScheduleDetails> CalculateAmortizationSchedules()
	{
		var schedules = new List<AmortizationScheduleDetails>();

		decimal extraPayment = 0;
		DateTime extraPaymentStartDate = DateTime.Now;

		foreach (var debt in _debts)
		{
			AmortizationScheduleDetails amortizationSchedule = new()
			{
				DebtId = debt.Id,
				ContractedMonthlyPayment = debt.MonthlyPayment,
				MonthlyDetails = new List<MonthlyAmortizationDetail> { CreateInitialMonthlyDetail(debt) }
			};

			do
			{
				// Determine the payment allocated to this debt for the month
				var allocatedPayment = debt.MonthlyPayment;

				if (extraPaymentStartDate <= DateTime.Now) allocatedPayment += extraPayment;


				var previousMonthDetail = amortizationSchedule.MonthlyDetails.Last();

				MonthlyAmortizationCalculator calculator = new(previousMonthDetail, allocatedPayment);
				MonthlyAmortizationDetail monthlyDetail = calculator.CalculateMonthlyDetail();

				amortizationSchedule.MonthlyDetails.Add(monthlyDetail);

				// Update the debt's remaining principal for the next iteration
				debt.RemainingPrincipal = monthlyDetail.DebtStateAtMonthEnd.RemainingPrincipal;

				// Update the extra payment start date to the next month
				extraPaymentStartDate = monthlyDetail.MonthYear.AddMonths(1);
			} while (debt.RemainingPrincipal > 0);

			// When a debt is paid off, its minimum payment is added to the extra payment for the next debts
			extraPayment += debt.MonthlyPayment;

			schedules.Add(amortizationSchedule);
		}

		return schedules;
	}

	public MonthlyAmortizationDetail CreateInitialMonthlyDetail(DebtDto debt)
	{
		return new MonthlyAmortizationDetail
		{
			DebtStateAtMonthEnd = debt
		};
	}
}
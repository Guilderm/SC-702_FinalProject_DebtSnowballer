using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public class AmortizationScheduleCalculator
{
	public List<AmortizationScheduleDetails> CalculateAmortizationSchedule(List<DebtDto> debts)
	{
		List<AmortizationScheduleDetails> schedules = new List<AmortizationScheduleDetails>();

		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate  = DateTime.Now.AddYears(45);

		foreach (DebtDto debt in debts)
		{
			AmortizationScheduleDetails amortizationSchedule = new()
			{
				DebtId = debt.Id,
				Auth0UserId = debt.Auth0UserId,
				NickName = debt.NickName,
				BankFees = debt.BankFees,
				ContractedMonthlyPayment = debt.MonthlyPayment,
				AnnualInterestRate = debt.MonthlyPayment,
				CurrencyCode = debt.CurrencyCode,
				CardinalOrder = debt.CardinalOrder,
				MonthlyDetails = new List<MonthlyAmortizationDetail> { CreateInitialMonthlyDetail(debt) }
			};

			do
			{
				MonthlyAmortizationDetail previousMonthDetail = amortizationSchedule.MonthlyDetails.Last();

				decimal allocatedPayment = 0;
				if (paymentReallocationStartDate <= previousMonthDetail.DebtStateAtMonthEnd.StartDate)
					allocatedPayment += paymentReallocationAmount;

				MonthlyAmortizationCalculator calculator = new(previousMonthDetail, allocatedPayment);
				MonthlyAmortizationDetail monthsDetail = calculator.CalculateMonthlyDetail();

				amortizationSchedule.MonthlyDetails.Add(monthsDetail);
				debt.RemainingPrincipal = monthsDetail.DebtStateAtMonthEnd.RemainingPrincipal;
			} while (debt.RemainingPrincipal > 0);

			// Actions done when a debt is paid off:
			MonthlyAmortizationDetail lastMonthDetail = amortizationSchedule.MonthlyDetails.Last();
			
			paymentReallocationStartDate = lastMonthDetail.DebtStateAtMonthEnd.StartDate;
			paymentReallocationAmount += amortizationSchedule.ContractedMonthlyPayment;

			amortizationSchedule.TotalBankFeesPaid = lastMonthDetail.AccumulatedBankFeesPaid;
			amortizationSchedule.TotalInterestPaid = lastMonthDetail.AccumulatedInterestPaid;
			amortizationSchedule.TotalPrincipalPaid = lastMonthDetail.AccumulatedPrincipalPaid;

			schedules.Add(amortizationSchedule);
		}

		return schedules;
	}

	private static MonthlyAmortizationDetail CreateInitialMonthlyDetail(DebtDto debt)
	{
		return new MonthlyAmortizationDetail
		{
			DebtStateAtMonthEnd = debt
		};
	}
}
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
		//DateTime extraPaymentStartDate = DateTime.Now.AddMonths(1);

		foreach (DebtDto debt in _debts)
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
				decimal allocatedPayment = amortizationSchedule.ContractedMonthlyPayment + extraPayment;
				//if (extraPaymentStartDate <= DateTime.Now) allocatedPayment += extraPayment;

				MonthlyAmortizationDetail previousMonthDetail = amortizationSchedule.MonthlyDetails.Last();

				MonthlyAmortizationCalculator calculator = new(previousMonthDetail, allocatedPayment);
				MonthlyAmortizationDetail monthlyDetail = calculator.CalculateMonthlyDetail();

				amortizationSchedule.MonthlyDetails.Add(monthlyDetail);
			} while (debt.RemainingPrincipal > 0);

			// When a debt is paid off, its minimum payment is added to the extra payment for the next debts
			extraPayment += debt.MonthlyPayment;

			MonthlyAmortizationDetail lastMonthDetail = amortizationSchedule.MonthlyDetails.Last();
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
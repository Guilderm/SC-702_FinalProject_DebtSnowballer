using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleCalculator
{
	public List<AmortizationScheduleDetails> CalculateAmortizationSchedule(List<LoanDetailDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		MonthlyAmortizationCalculator calculator = new();

		List<AmortizationScheduleDetails> schedules = new List<AmortizationScheduleDetails>();

		decimal allocatedPayment = 0;
		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate = DateTime.Now;
		//DateTime paymentReallocationStartDate = DateTime.Now.AddYears(45);

		foreach (LoanDetailDto debt in debts)
		{
			AmortizationScheduleDetails amortizationSchedule = CreateAmortizationScheduleDetails(debt);
			do
			{
				MonthlyAmortizationDetail amortizationAtMonthStart = amortizationSchedule.MonthlyDetails.Last();

				if (paymentReallocationStartDate <= amortizationAtMonthStart.LoanStateAtMonthEnd.StartDate)
				{
					Console.WriteLine($"Allocating reallocation amount: {paymentReallocationAmount}");
					allocatedPayment += paymentReallocationAmount;
				}

				MonthlyAmortizationDetail amortizationAtMonthEnd =
					calculator.CalculateMonthlyDetail(amortizationAtMonthStart, allocatedPayment);


				amortizationSchedule.MonthlyDetails.Add(amortizationAtMonthEnd);
				debt.RemainingPrincipal = amortizationAtMonthEnd.LoanStateAtMonthEnd.RemainingPrincipal;
			} while (debt.RemainingPrincipal > 0);

			Console.WriteLine($"Debt ID: {debt.Id} is paid off");

			// Actions done when a debt is paid off:
			MonthlyAmortizationDetail lastMonthDetail = amortizationSchedule.MonthlyDetails.Last();

			paymentReallocationStartDate = lastMonthDetail.LoanStateAtMonthEnd.StartDate;
			paymentReallocationAmount += amortizationSchedule.ContractedMonthlyPayment;

			amortizationSchedule.TotalBankFeesPaid = lastMonthDetail.AccumulatedBankFeesPaid;
			amortizationSchedule.TotalInterestPaid = lastMonthDetail.AccumulatedInterestPaid;
			amortizationSchedule.TotalPrincipalPaid = lastMonthDetail.AccumulatedPrincipalPaid;

			schedules.Add(amortizationSchedule);
		}

		Console.WriteLine($"Successfully calculated amortization schedules for {debts.Count} debts");
		return schedules;
	}

	private static AmortizationScheduleDetails CreateAmortizationScheduleDetails(LoanDetailDto loanDetail)
	{
		AmortizationScheduleDetails amortizationSchedule = new()
		{
			DebtId = loanDetail.Id,
			Auth0UserId = loanDetail.Auth0UserId,
			Name = loanDetail.Name,
			BankFees = loanDetail.BankFees,
			ContractedMonthlyPayment = loanDetail.ContractedMonthlyPayment,
			AnnualInterestRate = loanDetail.AnnualInterestRate,
			CurrencyCode = loanDetail.CurrencyCode,
			CardinalOrder = loanDetail.CardinalOrder,
			MonthlyDetails = new List<MonthlyAmortizationDetail> { CreateAmortizationDetailFromDebtDto(loanDetail) }
		};

		return amortizationSchedule;
	}

	private static MonthlyAmortizationDetail CreateAmortizationDetailFromDebtDto(LoanDetailDto loanDetail)
	{
		return new MonthlyAmortizationDetail
		{
			LoanStateAtMonthEnd = loanDetail
		};
	}
}
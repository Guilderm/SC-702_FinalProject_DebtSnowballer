using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.AmortizationScheduleService;

public class AmortizationScheduleCalculator
{
	public List<AmortizationScheduleDetails> CalculateAmortizationSchedule(List<DebtDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		List<AmortizationScheduleDetails> schedules = new List<AmortizationScheduleDetails>();

		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate = DateTime.Now.AddYears(45);

		foreach (DebtDto debt in debts)
		{
			Console.WriteLine($"Calculating amortization schedule for Debt ID: {debt.Id}");

			AmortizationScheduleDetails amortizationSchedule = CreateAmortizationScheduleDetails(debt);

			do
			{
				MonthlyAmortizationDetail previousMonthDetail = amortizationSchedule.MonthlyDetails.Last();

				decimal allocatedPayment = 0;
				if (paymentReallocationStartDate <= previousMonthDetail.DebtStateAtMonthEnd.StartDate)
				{
					Console.WriteLine($"Allocating reallocation amount: {paymentReallocationAmount}");
					allocatedPayment += paymentReallocationAmount;
				}

				MonthlyAmortizationCalculator calculator = new(previousMonthDetail, allocatedPayment);
				MonthlyAmortizationDetail monthsDetail = calculator.CalculateMonthlyDetail();

				amortizationSchedule.MonthlyDetails.Add(monthsDetail);
				debt.RemainingPrincipal = monthsDetail.DebtStateAtMonthEnd.RemainingPrincipal;
			} while (debt.RemainingPrincipal > 0);

			Console.WriteLine($"Debt ID: {debt.Id} is paid off");

			// Actions done when a debt is paid off:
			MonthlyAmortizationDetail lastMonthDetail = amortizationSchedule.MonthlyDetails.Last();

			paymentReallocationStartDate = lastMonthDetail.DebtStateAtMonthEnd.StartDate;
			paymentReallocationAmount += amortizationSchedule.ContractedMonthlyPayment;

			amortizationSchedule.TotalBankFeesPaid = lastMonthDetail.AccumulatedBankFeesPaid;
			amortizationSchedule.TotalInterestPaid = lastMonthDetail.AccumulatedInterestPaid;
			amortizationSchedule.TotalPrincipalPaid = lastMonthDetail.AccumulatedPrincipalPaid;

			schedules.Add(amortizationSchedule);
		}

		Console.WriteLine($"Successfully calculated amortization schedules for {debts.Count} debts");
		return schedules;
	}

	private static AmortizationScheduleDetails CreateAmortizationScheduleDetails(DebtDto debt)
	{
		AmortizationScheduleDetails amortizationSchedule = new()
		{
			DebtId = debt.Id,
			Auth0UserId = debt.Auth0UserId,
			NickName = debt.NickName,
			BankFees = debt.BankFees,
			ContractedMonthlyPayment = debt.MonthlyPayment,
			AnnualInterestRate = debt.AnnualInterestRate,
			CurrencyCode = debt.CurrencyCode,
			CardinalOrder = debt.CardinalOrder,
			MonthlyDetails = new List<MonthlyAmortizationDetail> { CreateAmortizationDetailFromDebtDTO(debt) }
		};

		Console.WriteLine("Amortization Schedule Details:");
		Console.WriteLine($"  DebtId: {amortizationSchedule.DebtId}");
		Console.WriteLine($"  Auth0UserId: {amortizationSchedule.Auth0UserId}");
		Console.WriteLine($"  NickName: {amortizationSchedule.NickName}");
		Console.WriteLine($"  BankFees: {amortizationSchedule.BankFees}");
		Console.WriteLine($"  ContractedMonthlyPayment: {amortizationSchedule.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {amortizationSchedule.AnnualInterestRate}");
		Console.WriteLine($"  CurrencyCode: {amortizationSchedule.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {amortizationSchedule.CardinalOrder}");
		Console.WriteLine($"  MonthlyDetails Count: {amortizationSchedule.MonthlyDetails.Count}");
		return amortizationSchedule;
	}

	private static MonthlyAmortizationDetail CreateAmortizationDetailFromDebtDTO(DebtDto debt)
	{
		Console.WriteLine($"Creating initial monthly detail for Debt ID: {debt.Id}");
		return new MonthlyAmortizationDetail
		{
			DebtStateAtMonthEnd = debt
		};
	}
}
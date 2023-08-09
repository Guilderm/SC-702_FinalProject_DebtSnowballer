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
			Console.WriteLine($"Calculating amortization schedule for Debt ID: {debt.Id}");
			Console.WriteLine("Details for Debt ID:");
			Console.WriteLine($"  Debt ID: {debt.Id}");
			Console.WriteLine($"  Auth0UserId: {debt.Auth0UserId}");
			Console.WriteLine($"  Name: {debt.Name}");
			Console.WriteLine($"  RemainingPrincipal: {debt.RemainingPrincipal}");
			Console.WriteLine($"  BankFees: {debt.BankFees}");
			Console.WriteLine($"  MonthlyPayment: {debt.ContractedMonthlyPayment}");
			Console.WriteLine($"  AnnualInterestRate: {debt.AnnualInterestRate}");
			Console.WriteLine($"  RemainingTermInMonths: {debt.RemainingTermInMonths}");
			Console.WriteLine($"  CurrencyCode: {debt.CurrencyCode}");
			Console.WriteLine($"  CardinalOrder: {debt.CardinalOrder}");
			Console.WriteLine($"  StartDate: {debt.StartDate:yyyy-MM-dd}");

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

		Console.WriteLine("Amortization Schedule Details:");
		Console.WriteLine($"  DebtId: {amortizationSchedule.DebtId}");
		Console.WriteLine($"  Auth0UserId: {amortizationSchedule.Auth0UserId}");
		Console.WriteLine($"  Name: {amortizationSchedule.Name}");
		Console.WriteLine($"  BankFees: {amortizationSchedule.BankFees}");
		Console.WriteLine($"  ContractedMonthlyPayment: {amortizationSchedule.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {amortizationSchedule.AnnualInterestRate}");
		Console.WriteLine($"  CurrencyCode: {amortizationSchedule.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {amortizationSchedule.CardinalOrder}");
		Console.WriteLine($"  MonthlyDetails Count: {amortizationSchedule.MonthlyDetails.Count}");
		return amortizationSchedule;
	}

	private static MonthlyAmortizationDetail CreateAmortizationDetailFromDebtDto(LoanDetailDto loanDetail)
	{
		Console.WriteLine("Creating initial monthly detail for Debt ID:");
		Console.WriteLine($"  Debt ID: {loanDetail.Id}");
		Console.WriteLine($"  Auth0UserId: {loanDetail.Auth0UserId}");
		Console.WriteLine($"  Name: {loanDetail.Name}");
		Console.WriteLine($"  RemainingPrincipal: {loanDetail.RemainingPrincipal}");
		Console.WriteLine($"  BankFees: {loanDetail.BankFees}");
		Console.WriteLine($"  MonthlyPayment: {loanDetail.ContractedMonthlyPayment}");
		Console.WriteLine($"  AnnualInterestRate: {loanDetail.AnnualInterestRate}");
		Console.WriteLine($"  RemainingTermInMonths: {loanDetail.RemainingTermInMonths}");
		Console.WriteLine($"  CurrencyCode: {loanDetail.CurrencyCode}");
		Console.WriteLine($"  CardinalOrder: {loanDetail.CardinalOrder}");
		Console.WriteLine($"  StartDate: {loanDetail.StartDate:yyyy-MM-dd}");


		return new MonthlyAmortizationDetail
		{
			LoanStateAtMonthEnd = loanDetail
		};
	}
}
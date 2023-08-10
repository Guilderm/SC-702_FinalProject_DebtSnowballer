using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class AmortizationScheduleCreator
{
	public List<AmortizationSchedule> CalculateAmortizationSchedule(List<LoanDetailDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		PaymentInstallmentCreator calculator = new();

		List<AmortizationSchedule> schedules = new List<AmortizationSchedule>();

		decimal allocatedPayment = 0;
		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate = DateTime.Now;
		//DateTime paymentReallocationStartDate = DateTime.Now.AddYears(45);

		foreach (LoanDetailDto debt in debts)
		{
			AmortizationSchedule amortizationSchedule = ConvertLoanDetailToAmortizationDetail(debt);
			do
			{
				/*
				Sorting is an O(n log n) operation, whereas finding the max value and then finding the first item with that value are both O(n) operations.
				But given that per business rules, the size of the list cannot be more than 540 (as each loan cannot be more than 45 years).
				The size of the list would not be sufficiently large to notice a real performance difference.
				So we chose sorting as it is a more concise and readable option.*/
				PaymentInstallment amortizationAtMonthStart =
					amortizationSchedule.MonthlyDetails.OrderByDescending(detail => detail.Month).First();


				if (paymentReallocationStartDate <= amortizationAtMonthStart.LoanStateAtMonthEnd.StartDate)
				{
					Console.WriteLine($"Allocating reallocation amount: {paymentReallocationAmount}");
					allocatedPayment += paymentReallocationAmount;
				}

				allocatedPayment = 0;

				PaymentInstallment amortizationAtMonthEnd =
					calculator.CalculateMonthlyDetail(amortizationAtMonthStart, allocatedPayment);


				amortizationSchedule.MonthlyDetails.Add(amortizationAtMonthEnd);
				debt.RemainingPrincipal = amortizationAtMonthEnd.LoanStateAtMonthEnd.RemainingPrincipal;

				Console.WriteLine(" loanAtMonthStart info is:");
				Console.WriteLine($"  Debt ID: {amortizationAtMonthEnd.LoanStateAtMonthEnd.Id}");
				Console.WriteLine($"  Auth0UserId: {amortizationAtMonthEnd.LoanStateAtMonthEnd.Auth0UserId}");
				Console.WriteLine($"  Name: {amortizationAtMonthEnd.LoanStateAtMonthEnd.Name}");
				Console.WriteLine(
					$"  RemainingPrincipal: {amortizationAtMonthEnd.LoanStateAtMonthEnd.RemainingPrincipal}");
				Console.WriteLine($"  BankFees: {amortizationAtMonthEnd.LoanStateAtMonthEnd.BankFees}");
				Console.WriteLine(
					$"  MonthlyPayment: {amortizationAtMonthEnd.LoanStateAtMonthEnd.ContractedMonthlyPayment}");
				Console.WriteLine(
					$"  AnnualInterestRate: {amortizationAtMonthEnd.LoanStateAtMonthEnd.AnnualInterestRate}");
				Console.WriteLine(
					$"  RemainingTermInMonths: {amortizationAtMonthEnd.LoanStateAtMonthEnd.RemainingTermInMonths}");
				Console.WriteLine($"  CurrencyCode: {amortizationAtMonthEnd.LoanStateAtMonthEnd.CurrencyCode}");
				Console.WriteLine($"  CardinalOrder: {amortizationAtMonthEnd.LoanStateAtMonthEnd.CardinalOrder}");
				Console.WriteLine($"  StartDate: {amortizationAtMonthEnd.LoanStateAtMonthEnd.StartDate:yyyy-MM-dd}");
				Console.WriteLine($"  StartDate: {amortizationAtMonthEnd.LoanStateAtMonthEnd.StartDate}");
			} while (debt.RemainingPrincipal > 0);

			Console.WriteLine($"Debt ID: {debt.Id} is paid off");

			// Actions done when a debt is paid off:
			PaymentInstallment lastMonthDetail = amortizationSchedule.MonthlyDetails.Last();

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

	private static AmortizationSchedule ConvertLoanDetailToAmortizationDetail(LoanDetailDto loanDetail)
	{
		AmortizationSchedule amortizationSchedule = new()
		{
			DebtId = loanDetail.Id,
			Auth0UserId = loanDetail.Auth0UserId,
			Name = loanDetail.Name,
			BankFees = loanDetail.BankFees,
			ContractedMonthlyPayment = loanDetail.ContractedMonthlyPayment,
			AnnualInterestRate = loanDetail.AnnualInterestRate,
			CurrencyCode = loanDetail.CurrencyCode,
			CardinalOrder = loanDetail.CardinalOrder,
			MonthlyDetails = new List<PaymentInstallment> { CreateAmortizationDetailFromDebtDto(loanDetail) }
		};

		return amortizationSchedule;
	}

	private static PaymentInstallment CreateAmortizationDetailFromDebtDto(LoanDetailDto loanDetail)
	{
		return new PaymentInstallment
		{
			LoanStateAtMonthEnd = new LoanDetailDto
			{
				Id = loanDetail.Id,
				Auth0UserId = loanDetail.Auth0UserId,
				Name = loanDetail.Name,
				RemainingPrincipal = loanDetail.RemainingPrincipal,
				BankFees = loanDetail.BankFees,
				ContractedMonthlyPayment = loanDetail.ContractedMonthlyPayment,
				AnnualInterestRate = loanDetail.AnnualInterestRate,
				RemainingTermInMonths = loanDetail.RemainingTermInMonths,
				CurrencyCode = loanDetail.CurrencyCode,
				CardinalOrder = loanDetail.CardinalOrder,
				StartDate = loanDetail.StartDate
			}
		};
	}
}
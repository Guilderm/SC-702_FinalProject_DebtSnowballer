using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class AmortizationScheduleCreator
{
	public List<AmortizationSchedule> CreateAmortizationSchedules(List<LoanDetailDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		PaymentInstallmentCreator paymentInstallmentCreator = new();

		List<AmortizationSchedule> amortizationSchedules = new List<AmortizationSchedule>();

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
				PaymentInstallment initialPaymentInstallment =
					amortizationSchedule.PaymentInstallments.OrderByDescending(detail => detail.PaymentMonth).First();


				if (paymentReallocationStartDate.Year < initialPaymentInstallment.EndOfMonthLoanState.StartDate.Year ||
				    (paymentReallocationStartDate.Year ==
				     initialPaymentInstallment.EndOfMonthLoanState.StartDate.Year &&
				     paymentReallocationStartDate.Month <=
				     initialPaymentInstallment.EndOfMonthLoanState.StartDate.Month))
				{
					Console.WriteLine($"Allocating reallocation amount: {paymentReallocationAmount}");
					allocatedPayment += paymentReallocationAmount;
				}

				allocatedPayment = 0;

				PaymentInstallment resultingPaymentInstallment =
					paymentInstallmentCreator.CalculateMonthlyDetail(initialPaymentInstallment, allocatedPayment);

				amortizationSchedule.PaymentInstallments.Add(resultingPaymentInstallment);
				Console.WriteLine(
					$"+++ RemainingPrincipal for loan {debt.Name} is: {resultingPaymentInstallment.EndOfMonthLoanState.RemainingPrincipal} as per resultingPaymentInstallment.EndOfMonthLoanState.RemainingPrincipal, and {debt.RemainingPrincipal} as per debt.RemainingPrincipal ");
				debt.RemainingPrincipal = resultingPaymentInstallment.EndOfMonthLoanState.RemainingPrincipal;

				Console.WriteLine(
					$"resulting Payment Installment Schedules for loan {resultingPaymentInstallment.EndOfMonthLoanState.Name} is:");
				Console.WriteLine($"  Debt ID: {resultingPaymentInstallment.EndOfMonthLoanState.Id}");
				Console.WriteLine($"  Name: {resultingPaymentInstallment.EndOfMonthLoanState.Name}");
				Console.WriteLine(
					$"  RemainingPrincipal: {resultingPaymentInstallment.EndOfMonthLoanState.RemainingPrincipal}");
				Console.WriteLine($"  BankFees: {resultingPaymentInstallment.EndOfMonthLoanState.BankFees}");
				Console.WriteLine(
					$"  MonthlyPayment: {resultingPaymentInstallment.EndOfMonthLoanState.ContractedMonthlyPayment}");
				Console.WriteLine(
					$"  RemainingTermInMonths: {resultingPaymentInstallment.EndOfMonthLoanState.RemainingTermInMonths}");
			} while (debt.RemainingPrincipal > 0);

			Console.WriteLine($"Debt ID: {debt.Id} is paid off");

			// Actions done when a debt is paid off:
			PaymentInstallment lastMonthDetail = amortizationSchedule.PaymentInstallments.Last();

			paymentReallocationStartDate = lastMonthDetail.EndOfMonthLoanState.StartDate;
			paymentReallocationAmount += amortizationSchedule.ContractedMonthlyPayment;

			amortizationSchedule.TotalBankFeesPaid = lastMonthDetail.AccumulatedBankFeesPaid;
			amortizationSchedule.TotalInterestPaid = lastMonthDetail.AccumulatedInterestPaid;
			amortizationSchedule.TotalPrincipalPaid = lastMonthDetail.AccumulatedPrincipalPaid;

			amortizationSchedules.Add(amortizationSchedule);
		}

		Console.WriteLine($"Successfully calculated amortization schedules for {debts.Count} debts");
		return amortizationSchedules;
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
			PaymentInstallments = new List<PaymentInstallment> { CreateAmortizationDetailFromDebtDto(loanDetail) }
		};

		return amortizationSchedule;
	}

	private static PaymentInstallment CreateAmortizationDetailFromDebtDto(LoanDetailDto loanDetail)
	{
		return new PaymentInstallment
		{
			EndOfMonthLoanState = new LoanDetailDto
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
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class AmortizationScheduleCreator
{
	public List<AmortizationSchedule> CreateAmortizationSchedules(List<LoanDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		PaymentInstallmentCreator paymentInstallmentCreator = new();

		List<AmortizationSchedule> amortizationSchedules = new List<AmortizationSchedule>();

		decimal allocatedPayment = 0;
		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate = DateTime.Now;
		//DateTime paymentReallocationStartDate = DateTime.Now.AddYears(45);

		var debtsSummary = debts.Select(d =>
			$"Id: {d.Id}, " +
			$"Name: {d.Name}, " +
			$"Principal: ${d.RemainingPrincipal:0.00}, " +
			$"Bank Fees: ${d.BankFees:0.00}, " +
			$"Monthly Payment: ${d.ContractedMonthlyPayment:0.00}, " +
			$"Interest Rate: {d.AnnualInterestRate:P2}, " +
			$"Term: {d.RemainingTermInMonths} months, " +
			$"Currency: {d.CurrencyCode}, " +
			$"Order: {d.CardinalOrder}, " +
			$"Start Date: {d.StartDate:yyyy-MM-dd}"
		).ToList();

		Console.WriteLine("Entered function 'CalculatePaymentPlansAsync'");
		Console.WriteLine("Debts:");
		foreach (string debtSummary in debtsSummary) Console.WriteLine($"- {debtSummary}");


		foreach (LoanDto debt in debts)
		{
			Console.WriteLine($"the date for loan {debt.Name} in 'AmortizationScheduleCreator' is: {debt.StartDate}");
			AmortizationSchedule amortizationSchedule = ConvertLoanDetailToAmortizationDetail(debt);
			do
			{
				PaymentInstallment initialPaymentInstallment = amortizationSchedule.PaymentInstallments.Last();
				Console.WriteLine(
					$"Retrieve PaymentInstallment, month {initialPaymentInstallment.PaymentMonth} for {initialPaymentInstallment.EndOfMonthLoanState.Name} whit date of {initialPaymentInstallment.EndOfMonthLoanState.StartDate}");

				if (paymentReallocationStartDate.Year < initialPaymentInstallment.EndOfMonthLoanState.StartDate.Year ||
				    (paymentReallocationStartDate.Year ==
				     initialPaymentInstallment.EndOfMonthLoanState.StartDate.Year &&
				     paymentReallocationStartDate.Month <=
				     initialPaymentInstallment.EndOfMonthLoanState.StartDate.Month))
				{
					allocatedPayment += paymentReallocationAmount;
					Console.WriteLine(
						$"Allocating reallocation for the amount of: {paymentReallocationAmount}, allocatedPayment is now {allocatedPayment}");
				}

				PaymentInstallment resultingPaymentInstallment =
					paymentInstallmentCreator.CreatePaymentInstallment(initialPaymentInstallment, allocatedPayment);

				amortizationSchedule.PaymentInstallments.Add(resultingPaymentInstallment);
				debt.RemainingPrincipal = resultingPaymentInstallment.EndOfMonthLoanState.RemainingPrincipal;

				// A amount that is below 0.01 is too low to give a meaning full result, so we will act as if the Remaining Principal is 0
			} while (debt.RemainingPrincipal > (decimal)0.001);

			Console.WriteLine($"Debt ID: {debt.Id} Name: {debt.Name} is paid off");

			PaymentInstallment lastMonthDetail = amortizationSchedule.PaymentInstallments.Last();

			paymentReallocationStartDate = lastMonthDetail.EndOfMonthLoanState.StartDate;
			paymentReallocationAmount += amortizationSchedule.ContractedMonthlyPayment;

			amortizationSchedule.TotalBankFeesPaid = lastMonthDetail.AccumulatedBankFeesPaid;
			amortizationSchedule.TotalInterestPaid = lastMonthDetail.AccumulatedInterestPaid;
			amortizationSchedule.TotalPrincipalPaid = lastMonthDetail.AccumulatedPrincipalPaid;
			amortizationSchedule.TotalExtraPayment = lastMonthDetail.AccumulatedExtraPayment;

			amortizationSchedules.Add(amortizationSchedule);
		}

		Console.WriteLine($"Successfully calculated amortization schedules for {debts.Count} debts");
		return amortizationSchedules;
	}

	private static AmortizationSchedule ConvertLoanDetailToAmortizationDetail(LoanDto loan)
	{
		AmortizationSchedule amortizationSchedule = new()
		{
			DebtId = loan.Id,
			Auth0UserId = loan.Auth0UserId,
			Name = loan.Name,
			BankFees = loan.BankFees,
			ContractedMonthlyPayment = loan.ContractedMonthlyPayment,
			AnnualInterestRate = loan.AnnualInterestRate,
			CurrencyCode = loan.CurrencyCode,
			CardinalOrder = loan.CardinalOrder,
			PaymentInstallments = new List<PaymentInstallment> { CreateAmortizationDetailFromDebtDto(loan) }
		};

		return amortizationSchedule;
	}

	private static PaymentInstallment CreateAmortizationDetailFromDebtDto(LoanDto loan)
	{
		return new PaymentInstallment
		{
			EndOfMonthLoanState = new LoanDto
			{
				Id = loan.Id,
				Auth0UserId = loan.Auth0UserId,
				Name = loan.Name,
				RemainingPrincipal = loan.RemainingPrincipal,
				BankFees = loan.BankFees,
				ContractedMonthlyPayment = loan.ContractedMonthlyPayment,
				AnnualInterestRate = loan.AnnualInterestRate,
				RemainingTermInMonths = loan.RemainingTermInMonths,
				CurrencyCode = loan.CurrencyCode,
				CardinalOrder = loan.CardinalOrder,
				StartDate = loan.StartDate
			}
		};
	}
}
using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class LoanAmortizationCreator
{
	private readonly IPaymentInstallmentFactory _paymentInstallmentFactory;


	public LoanAmortizationCreator(IPaymentInstallmentFactory paymentInstallmentFactory)
	{
		_paymentInstallmentFactory = paymentInstallmentFactory;
		Console.WriteLine("LoanAmortizationCreator initialized with PaymentInstallmentCreator");
	}

	public List<LoanAmortization> CreateAmortizationSchedules(List<LoanDto> debts)
	{
		Console.WriteLine($"Entered function 'CalculateAmortizationSchedule' with {debts.Count} debts");

		List<LoanAmortization> amortizationSchedules = new();

		decimal allocatedPayment = 0;
		decimal paymentReallocationAmount = 0;
		DateTime paymentReallocationStartDate = DateTime.Now;

		foreach (LoanDto loan in debts)
		{
			Console.WriteLine(
				$"the date for loan {loan.Name} with ID: {loan.Id} in 'LoanAmortizationCreator' is: {loan.StartDate}");
			LoanAmortization amortization = ConvertLoanDetailToAmortizationDetail(loan);
			PaymentInstallmentCreator paymentInstallmentCreator = _paymentInstallmentFactory.Create(
				amortization.ContractedMonthlyPayment,
				amortization.AnnualInterestRate, amortization.BankFees);
			do
			{
				PaymentInstallment initialPaymentInstallment = amortization.Schedule.Last();
				Console.WriteLine(
					$"Retrieve PaymentInstallment, month {initialPaymentInstallment.Month} for {initialPaymentInstallment.Name} whit date of {initialPaymentInstallment.Date}");

				if (paymentReallocationStartDate.Year < initialPaymentInstallment.Date.Year ||
				    (paymentReallocationStartDate.Year == initialPaymentInstallment.Date.Year &&
				     paymentReallocationStartDate.Month <= initialPaymentInstallment.Date.Month))
				{
					allocatedPayment += paymentReallocationAmount;
					Console.WriteLine(
						$"Allocating reallocation for the amount of: {paymentReallocationAmount}, allocatedPayment is now {allocatedPayment}");
				}

				PaymentInstallment resultingPaymentInstallment =
					paymentInstallmentCreator.CreatePaymentInstallment(initialPaymentInstallment, allocatedPayment);

				amortization.Schedule.Add(resultingPaymentInstallment);
				loan.RemainingPrincipal = resultingPaymentInstallment.RemainingPrincipal;

				// A amount that is below 0.01 is too low to give a meaning full result, so we will act as if the Remaining Principal is 0
			} while (loan.RemainingPrincipal > (decimal)0.001);

			Console.WriteLine($"Debt ID: {loan.Id} Name: {loan.Name} is paid off");

			PaymentInstallment lastMonthDetail = amortization.Schedule.Last();

			paymentReallocationStartDate = lastMonthDetail.Date;
			paymentReallocationAmount += amortization.ContractedMonthlyPayment;

			amortization.TotalBankFeesPaid = lastMonthDetail.TotalBankFeesPaid;
			amortization.TotalInterestPaid = lastMonthDetail.TotalInterestPaid;
			amortization.TotalPrincipalPaid = lastMonthDetail.TotalPrincipalPaid;
			amortization.TotalExtraPayment = lastMonthDetail.TotalExtraPayment;

			amortizationSchedules.Add(amortization);
		}

		Console.WriteLine($"Successfully calculated amortization schedules for {debts.Count} debts");
		return amortizationSchedules;
	}

	private static LoanAmortization ConvertLoanDetailToAmortizationDetail(LoanDto loan)
	{
		LoanAmortization loanAmortization = new()
		{
			DebtId = loan.Id,
			Name = loan.Name,
			BankFees = loan.BankFees,
			ContractedMonthlyPayment = loan.ContractedMonthlyPayment,
			AnnualInterestRate = loan.AnnualInterestRate,
			CurrencyCode = loan.CurrencyCode,
			CardinalOrder = loan.CardinalOrder,
			Schedule = new List<PaymentInstallment> { CreateAmortizationDetailFromDebtDto(loan) }
		};

		return loanAmortization;
	}

	private static PaymentInstallment CreateAmortizationDetailFromDebtDto(LoanDto loan)
	{
		return new PaymentInstallment
		{
			LoanId = loan.Id,
			Name = loan.Name,
			RemainingPrincipal = loan.RemainingPrincipal,
			RemainingTermInMonths = loan.RemainingTermInMonths,
			Date = loan.StartDate
		};
	}
}
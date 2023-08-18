using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class DebtPayoffPlanCreator
{
	private readonly AmortizationScheduleCreator _scheduleCreator;


	private readonly DebtPayoffPlan _debtPayoffPlan = new();

	public DebtPayoffPlanCreator(AmortizationScheduleCreator amortizationScheduleCreator)
	{
		_scheduleCreator = amortizationScheduleCreator ??
		                   throw new ArgumentNullException(nameof(amortizationScheduleCreator));
		Console.WriteLine("DebtPayoffPlanCreator initialized with AmortizationScheduleCreator");
	}

	public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDto> debts)
	{
		if (debts == null)
			throw new ArgumentNullException(nameof(debts));

		Console.WriteLine($"Entered function 'CalculatePaymentPlansAsync' with debts count: {debts.Count}");


		//await CalculateAndAddPaymentPlan( "Baseline", debts, d => d.Id);

		await CalculateAndAddPaymentPlan("Snowball", debts, d => d.RemainingPrincipal);

		await CalculateAndAddPaymentPlan("Avalanche", debts, d => d.AnnualInterestRate);

		//await CalculateAndAddPaymentPlan( "Custom", debts, d => d.CardinalOrder);

		Console.WriteLine("Successfully calculated all payment plans");
		return _debtPayoffPlan;
	}

	private async Task CalculateAndAddPaymentPlan(string planName,
		List<LoanDto> debts, Func<LoanDto, object> orderBy)
	{
		Console.WriteLine($"Calculating {planName} payment plan...");

		var sortedDebts = DeepCopy(debts).OrderByDescending(orderBy).ToList();
		_debtPayoffPlan.PaymentPlans[planName] =
			await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(sortedDebts));
		Console.WriteLine($"payment plan {planName}  was calculated");
	}

	private List<LoanDto> DeepCopy(List<LoanDto> original)
	{
		return original.Select(item => new LoanDto
		{
			Id = item.Id,
			Auth0UserId = item.Auth0UserId,
			Name = item.Name,
			RemainingPrincipal = item.RemainingPrincipal,
			BankFees = item.BankFees,
			ContractedMonthlyPayment = item.ContractedMonthlyPayment,
			AnnualInterestRate = item.AnnualInterestRate,
			RemainingTermInMonths = item.RemainingTermInMonths,
			CurrencyCode = item.CurrencyCode,
			CardinalOrder = item.CardinalOrder,
			StartDate = item.StartDate
		}).ToList();
	}
}
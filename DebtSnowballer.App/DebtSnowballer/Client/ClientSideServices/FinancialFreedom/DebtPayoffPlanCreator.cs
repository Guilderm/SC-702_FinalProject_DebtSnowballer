using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public class DebtPayoffPlanCreator
{
    private readonly AmortizationScheduleCreator _scheduleCreator;

    public DebtPayoffPlanCreator(AmortizationScheduleCreator amortizationScheduleCreator)
    {
        _scheduleCreator = amortizationScheduleCreator ??
                           throw new ArgumentNullException(nameof(amortizationScheduleCreator));
        Console.WriteLine("DebtPayoffPlanCreator initialized with AmortizationScheduleCreator");
    }

    public async Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDetailDto> debts)
    {
        if (debts == null)
            throw new ArgumentNullException(nameof(debts));

        Console.WriteLine($"Entered function 'CalculatePaymentPlansAsync' with debts count: {debts.Count}");

        DebtPayoffPlan debtPayoffPlan = new();

        await CalculateAndAddPaymentPlan(debtPayoffPlan, "Baseline", debts, d => d.Id);

        await CalculateAndAddPaymentPlan(debtPayoffPlan, "Snowball", debts, d => d.RemainingPrincipal);

        await CalculateAndAddPaymentPlan(debtPayoffPlan, "Avalanche", debts, d => d.AnnualInterestRate);

        await CalculateAndAddPaymentPlan(debtPayoffPlan, "Custom", debts, d => d.CardinalOrder);

        Console.WriteLine("Successfully calculated all payment plans");
        return debtPayoffPlan;
    }

    private async Task CalculateAndAddPaymentPlan(DebtPayoffPlan debtPayoffPlan, string planName,
        List<LoanDetailDto> debts, Func<LoanDetailDto, object> orderBy)
    {
        Console.WriteLine($"Calculating {planName} payment plan...");
        var sortedDebts = DeepCopy(debts).OrderByDescending(orderBy).ToList();

        debtPayoffPlan.PaymentPlans[planName] =
            await Task.Run(() => _scheduleCreator.CreateAmortizationSchedules(sortedDebts));
        Console.WriteLine($"payment plan {planName}  was calculated");
    }

    private List<LoanDetailDto> DeepCopy(List<LoanDetailDto> original)
    {
        return original.Select(item => new LoanDetailDto
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
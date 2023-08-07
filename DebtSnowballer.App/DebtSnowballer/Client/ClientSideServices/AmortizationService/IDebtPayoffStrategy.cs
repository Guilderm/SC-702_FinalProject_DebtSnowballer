namespace DebtSnowballer.Client.ClientSideServices.AmortizationService;

public interface IDebtPayoffStrategy
{
	PaymentPlanDetails CalculateStrategy(List<Debt> debts);
}
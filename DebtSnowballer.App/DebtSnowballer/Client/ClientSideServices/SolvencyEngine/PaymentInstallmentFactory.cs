namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public class PaymentInstallmentFactory : IPaymentInstallmentFactory
{
	public PaymentInstallmentCreator Create(decimal contractedMonthlyPayment, decimal annualInterestRate,
		decimal monthlybankfees)
	{
		return new PaymentInstallmentCreator(contractedMonthlyPayment, annualInterestRate, monthlybankfees);
	}
}
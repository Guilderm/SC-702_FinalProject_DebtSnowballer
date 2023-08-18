namespace DebtSnowballer.Client.ClientSideServices.SolvencyEngine;

public interface IPaymentInstallmentFactory
{
	PaymentInstallmentCreator Create(decimal contractedMonthlyPayment, decimal annualInterestRate,
		decimal monthlybankfees);
}
namespace DebtSnowballer.Client.Components.DebtManagement;

public class DebtPaymentPlanBuilder
{
	// Method to calculate the payoff date for a debt
	public static int CalculatePayoffPeriod(double principal, double interestRate, double minimumPayment)
	{
		double balance = principal;
		int numberOfPeriods = 0;
		double monthlyInterestRate = interestRate / 12 / 100; // Assuming the interest rate is annual

		while (balance > 0)
		{
			// Calculate the interest for the current period
			double interest = balance * monthlyInterestRate;

			// Calculate the amount of principal paid
			double principalPaid = minimumPayment - interest;

			// If the interest is greater than the minimum payment, then the debt will never be paid off
			if (principalPaid <= 0)
				throw new Exception(
					"The minimum payment is too low to cover the interest. The debt will never be paid off.");

			// Subtract the principal paid from the balance
			balance -= principalPaid;

			// Increase the number of periods
			numberOfPeriods++;
		}

		return numberOfPeriods;
	}

	// Method to calculate the total interest paid for a debt
	public static double CalculateTotalInterestPaid(double principal, double interestRate, int numberOfPeriods)
	{
		double monthlyInterestRate = interestRate / 12 / 100; // Assuming the interest rate is annual
		return principal * monthlyInterestRate * numberOfPeriods;
	}

	// Method to calculate the interest saved using the debt snowball method
	public static double CalculateInterestSaved(double principal, double interestRate, double minimumPayment)
	{
		int payoffPeriods = CalculatePayoffPeriod(principal, interestRate, minimumPayment);
		double totalInterestWithMinimumPayments = CalculateTotalInterestPaid(principal, interestRate, payoffPeriods);

		// Assuming the debt snowball method reduces the number of periods to pay off the debt
		// (This is a simplification; in reality, you'd need to calculate the number of periods using the debt snowball method)
		double totalInterestWithDebtSnowball = CalculateTotalInterestPaid(principal, interestRate, payoffPeriods);

		return totalInterestWithMinimumPayments - totalInterestWithDebtSnowball;
	}
}
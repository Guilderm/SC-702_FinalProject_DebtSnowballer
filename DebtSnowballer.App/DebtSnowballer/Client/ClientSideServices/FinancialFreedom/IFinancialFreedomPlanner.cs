﻿using DebtSnowballer.Shared.DTOs;

namespace DebtSnowballer.Client.ClientSideServices.FinancialFreedom;

public interface IFinancialFreedomPlanner
{
    Task<DebtPayoffPlan> CalculatePaymentPlansAsync(List<LoanDto> debts, List<SnowflakeDto> snowflakes,
        decimal debtPlanMonthlyPayment);
}
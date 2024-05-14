using System;
using UnityEngine;
public class Loan
{
    public int amount;
    public double interest;
    public int LoanTermYears;
    public int CalculateMonthlyPayment()
    {
        double monthlyInterestRate = interest / 12.0 / 100.0; // Convert annual interest rate to monthly and percentage to decimal
        int numberOfPayments = LoanTermYears * 12; // Convert loan term from years to months
        // Calculate the monthly payment using the fixed-rate mortgage formula
        double monthlyPayment = amount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, numberOfPayments) /
                                (Math.Pow(1 + monthlyInterestRate, numberOfPayments) - 1);
        return Convert.ToInt32(monthlyPayment);
    }
    public void Player_Pay_Loan(PlayerMoney p)
    {
        int MonthlyPay = Math.Min(CalculateMonthlyPayment(), amount);
        p.Money -= MonthlyPay;
        amount -= MonthlyPay;
    }
}
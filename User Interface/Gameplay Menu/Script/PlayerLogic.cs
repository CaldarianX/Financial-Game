using System;
using System.Collections.Generic;
using System.Linq;
public class PlayerMoney
{
    // logic3 logic;

    public int house = 0;
    public List<House> Player_House = new List<House>();
    public List<Condo> Player_Condo = new List<Condo>();
    public List<Warehouse> Player_Warehouse = new List<Warehouse>();
    public List<Loan> Player_Loan = new List<Loan>();
    public List<Stock> Player_Stock = new List<Stock>();
    public int condo = 0;
    public string name = "";
    public int warehouse = 0;
    public int Net_Worth = 0;
    public int Money = 0;
    public int Salary = 30000;
    // public string name = "";
    // public Dictionary<string, Dictionary<float, int>> Player_Stock = new Dictionary<string, Dictionary<float, int>>();
    public void Turn()
    {
        Money += Salary;
    }
    public void Buy_Stock(string name, int amount, float price)
    {
        foreach (var stock in Player_Stock)
        {
            if (stock.name == name && stock.price == price)
            {
                stock.amount += amount;
                Money -= Convert.ToInt32(price * amount);
                return;
            }
        }
        Stock new_stock = new Stock()
        {
            name = name,
            amount = amount,
            price = price,
        };
        // Player_Stock[name][price] += amount;
        Player_Stock.Add(new_stock);
        Player_Stock.OrderByDescending(pair => pair.name).ToList();
        Money -= Convert.ToInt32(price * amount);
        return;
    }
    public string Show_Player_Stock()
    {
        string Show = "";
        foreach (var stock in Player_Stock)
        {
            string formattedPrice = stock.price.ToString("F2");
            Show += stock.name + " x " + stock.amount + "  " + formattedPrice + "\n";
        }
        return Show;
    }
    public void Calculate_Net_Worth(Dictionary<string, Stock> Stocks_Data)
    {
        int worth = Money;
        foreach (var stock in Player_Stock)
        {
            worth += Convert.ToInt32(Stocks_Data[stock.name].price * stock.amount);
        }
        Net_Worth = worth;
    }
    public void Add_Player_Loan(Loan l)
    {
        Player_Loan.Add(l);
    }
    public void Pay_Loan()
    {
        foreach (var Loan in Player_Loan)
        {
            Loan.Player_Pay_Loan(this);
            if (Loan.amount <= 0)
            {
                Player_Loan.Remove(Loan);
            }
        }
    }
}
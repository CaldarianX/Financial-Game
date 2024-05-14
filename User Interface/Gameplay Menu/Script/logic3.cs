// using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class logic3 : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Real_Estate_Card;
    public GameObject Real_Estate_CloseButton;
    public GameObject Stock_Market_Card;
    public GameObject Stock_Market_CloseButton;
    public GameObject Timer_Bar_Turn;
    public GameObject Stock_Market_Buy_Menu;
    public GameObject Stock_Market_Input_Amount;
    public GameObject Real_Estate_Buy_Menu;
    public GameObject Random_Event_Card;
    public GameObject Random_Event_Card_Message;
    public GameObject House_Icon_Grid;
    public GameObject House_Icon;
    public GameObject Condo_Icon_Grid;
    public GameObject Condo_Icon;
    public GameObject Warehouse_Icon_Grid;
    public GameObject Warehouse_Icon;
    public GameObject Bank_Menu;
    public Text Bank_Borrow_Money_Input;
    public GameObject Bank_Monthly_Pay;
    public GameObject[] Arrayplayer = new GameObject[4];
    public GameObject[] Stock_List_UI = new GameObject[5];
    public GameObject[] Player_name_UI = new GameObject[4];
    public GameObject[] House_List_UI = new GameObject[5];
    public GameObject[] Condo_List_UI = new GameObject[5];
    public GameObject[] Warehouse_List_UI = new GameObject[5];
    public TMP_Text ScoreBoard_UI;
    public TMP_Text Portfolio;
    public TMP_Text Player_Balance;
    Dictionary<string, PlayerMoney> Players_Data = new Dictionary<string, PlayerMoney>();
    Dictionary<string, Stock> Stocks_Data = new Dictionary<string, Stock>();
    House[] House_Detail = new House[5];
    Condo[] Condo_Detail = new Condo[5];
    Warehouse[] Warehouse_Detail = new Warehouse[5];
    Vector2[] BoardPosition = new Vector2[4];
    Logic2 SelectMenuLogic;
    int index = -1;
    int selected_house;
    int selected_condo;
    int selected_warehouse;
    int NumberPlayer;
    int realestatebuyMode = -1;
    int RoundCount = 0;
    bool[] PlayerDetail = new bool[4];
    bool IsTimer = true;
    string[] Namelist = { "P1", "P2", "P3", "P4" };
    string[] StockList = { "CPALL", "GULF", "PTT", "ADVANC", "BTS" };
    string[] eventmessage = { "Right now, the economy is getting better", "Now, the economy is going down." };
    string[] realestate_location = { "Bangkok", "Saputprakran", "Rayong", "ChingMai" };
    string Selected_Stock_Name = "";
    float[] StockPrice = { 59.00f, 49.00f, 13.25f, 206f, 6.10f };
    float[] StockBit = { 0.25f, 0.25f, 0.25f, 1f, 0.05f };
    float[] BoardRotation = new float[4];
    float stockup = 1.0f;
    float stockdown = 1.0f;
    float TimeEachTurn = 30;
    float Timer = 0;



    // Dictionary<string, int> ScoreBoard = new Dictionary<string, int>();
    void Start()
    {
        SelectMenuLogic = FindAnyObjectByType<Logic2>();
        NumberPlayer = SelectMenuLogic.PlayerNumber;
        PlayerDetail = SelectMenuLogic.PlayerDetail();
        // Debug.Log("Player: " + NumberPlayer.ToString());
        for (int i = 0; i < 4; i++)
        {
            Arrayplayer[i].SetActive(PlayerDetail[i]);
            if (PlayerDetail[i])
            {
                Players_Data[Namelist[i]] = new PlayerMoney
                {
                    name = Namelist[i],
                    Salary = 100000,

                };
                Players_Data[Namelist[i]].Turn();
            }
        }

        for (int i = 0; i < StockList.Length; i++)
        {
            Stocks_Data[StockList[i]] = new Stock()
            {
                name = StockList[i],
                price = StockPrice[i],
                bit = StockBit[i],
            };
        }
        RealEstate_UI_List();
        Update_ScoreBoard();
        BoardPosition[0] = new Vector3(33f, -36f);
        BoardPosition[1] = new Vector3(-180f, -25f);
        BoardPosition[2] = new Vector3(-8f, 42f);
        BoardPosition[3] = new Vector3(186f, 16f);
        BoardRotation[0] = 0f;
        BoardRotation[1] = 270f;
        BoardRotation[2] = 180f;
        BoardRotation[3] = 90f;
        NextTurn();
        Update_Player_Balance();

    }
    // Update is called once per frame
    void Update()
    {
        if (IsTimer)
        {
            Timer += Time.deltaTime;
        }

        if (Timer > TimeEachTurn)
        {
            Reset_Timer_UI();
            NextTurn();
            Load_player_Portolio();
            Update_Player_Balance();
            RoundCount++;
        }
        //generate price;
        if (NumberPlayer == RoundCount)
        {
            Give_Player_Salary();
            Update_Net_worth();
            Update_ScoreBoard();
            Random_Event_Card_Logic();
            RealEstate_UI_List();
            foreach (var stock in Stocks_Data)
            {
                stock.Value.price = Generate_Stock_Price(stock.Value.price, stock.Value.bit);
            }
            for (int i = 0; i < StockPrice.Length; i++)
            {
                Stock_List_UI[i].GetComponent<TMP_Text>().text = StockList[i] + "  " + Stocks_Data[StockList[i]].price.ToString("F2");
                // Debug.Log(StockList[i] + "  " + StockPrice[i].ToString("F2"));
            }
            RoundCount = 0;
            foreach (var player in Players_Data)
            {
                player.Value.Pay_Loan();
            }

        }
        Timer_UI(Timer, TimeEachTurn, Timer_Bar_Turn);
        // Players_Data[Namelist[index]] = Players_Data[Namelist[index]];
    }
    public void NextTurn()
    {
        Timer = 0;
        int diff = 0;
        while (true)
        {
            diff++;
            index = (index + 1) % 4;
            if (PlayerDetail[index])
            {
                break;
            }
        }
        Update_Net_worth();
        Update_ScoreBoard();
    }
    public void Timer_UI(float t1, float t2, GameObject Bar)
    {
        float percent = t1 / t2;
        RectTransform rectTransform = Bar.GetComponent<RectTransform>();
        float timerWidth = (1 - percent) * 686;
        Vector2 originalSizeDelta = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(timerWidth, originalSizeDelta.y);
        // Debug.Log(t1 + " " + t2 + " === " + percent);
    }
    public void Reset_Timer_UI()
    {
        Timer_Bar_Turn.GetComponent<RectTransform>().sizeDelta = new Vector2(686, 17);
    }
    public void OpenRealEstate()
    {
        ToggleButton(Real_Estate_CloseButton, Real_Estate_Card, true);
    }
    public void CloseRealEstate()
    {
        ToggleButton(Real_Estate_CloseButton, Real_Estate_Card, false);
    }
    public void OpenStockMarket()
    {
        ToggleButton(Stock_Market_CloseButton, Stock_Market_Card, true);
    }
    public void CloseStockMarket()
    {
        ToggleButton(Stock_Market_CloseButton, Stock_Market_Card, false);
        Stock_Market_Buy_Menu.SetActive(false);
    }
    public void Open_Stock_Menu_Logic(string name)
    {
        Stock_Market_Buy_Menu.SetActive(true);
        Selected_Stock_Name = name;
    }
    public void Close_Stock_Menu_Logic()
    {
        Stock_Market_Buy_Menu.SetActive(false);
    }
    public void ToggleButton(GameObject tmp1, GameObject tmp2, bool Isactive)
    {
        tmp1.SetActive(Isactive);
        tmp2.SetActive(Isactive);
    }
    public void BackButton()
    {
        SceneManager.LoadScene(1);
        Destroy(SelectMenuLogic.gameObject);
    }
    public void Buy_Stock_Button()
    {
        Debug.Log(Players_Data[Namelist[index]]);
        Debug.Log(Selected_Stock_Name);
        Debug.Log(int.Parse(Stock_Market_Input_Amount.GetComponent<InputField>().text));
        Debug.Log(Stocks_Data[Selected_Stock_Name].price);
        Players_Data[Namelist[index]].Buy_Stock(Selected_Stock_Name, int.Parse(Stock_Market_Input_Amount.GetComponent<InputField>().text), Stocks_Data[Selected_Stock_Name].price);
        Load_player_Portolio();
        Update_ScoreBoard();
        Update_Player_Balance();
    }
    public void Load_player_Portolio()
    {
        string port = Players_Data[Namelist[index]].Show_Player_Stock();
        Portfolio.text = port;
    }
    public float Generate_Stock_Price(float price, float bit)
    {
        int numberbit = Convert.ToInt32(price / 10 / bit);
        int Randoms = UnityEngine.Random.Range(Convert.ToInt32(-1 * numberbit * stockdown), Convert.ToInt32(numberbit * stockup));
        float DiffPrice = Randoms * bit;
        return price + DiffPrice;
    }
    public void Update_ScoreBoard()

    {
        var SortedMoney = Players_Data.OrderByDescending(pair => pair.Value.Net_Worth);
        string Show = "";
        foreach (var player in SortedMoney)
        {
            Show += player.Value.name + " : " + player.Value.Net_Worth.ToString() + "\n";
        }
        ScoreBoard_UI.text = Show;
    }
    public void TimerUpdate(bool Ison)
    {
        IsTimer = Ison;
    }
    public void Random_Event_Card_Logic()
    {
        TimerUpdate(false);
        int state = UnityEngine.Random.Range(1, 2);
        if (state == 0) return;
        int random_message = UnityEngine.Random.Range(0, eventmessage.Length);
        if (random_message == 0)
        {
            stockup = 0.7f;
            stockdown = 0.3f;
        }
        if (random_message == 1)
        {
            stockup = 0.3f;
            stockdown = 0.7f;
        }
        Random_Event_Card.SetActive(true);
        Random_Event_Card_Message.GetComponent<TMP_Text>().text = eventmessage[random_message];
    }
    public void Close_Random_Event_Card()
    {
        TimerUpdate(true);
        Random_Event_Card.SetActive(false);
    }
    public void Update_Net_worth()
    {
        foreach (var player in Players_Data)
        {
            player.Value.Calculate_Net_Worth(Stocks_Data);
        }
    }
    public void Give_Player_Salary()
    {
        foreach (var player in Players_Data)
        {
            player.Value.Turn();
        }
    }
    public void Skip_Turn()
    {
        Timer = TimeEachTurn;
    }
    public void Update_Player_Balance()
    {
        Debug.Log(Players_Data[Namelist[index]].Money);
        Debug.Log(Player_Balance);
        Player_Balance.text = "Balance : " + Players_Data[Namelist[index]].Money.ToString();
    }
    public void RealEstate_UI_List()
    {
        Generate_Condo_Detail();
        Generate_House_Detail();
        Generate_Warehouse_Detail();
        Load_Condo_Detail();
        Load_House_Detail();
        Load_Warehouse_Detail();
    }
    public void Generate_House_Detail()
    {
        // int[] location_price = { 200000, 100000, 50000, 50000 };
        for (int i = 0; i < 5; i++)
        {
            int Hsize = UnityEngine.Random.Range(150, 400);
            int Hbedroom = UnityEngine.Random.Range(2, 5);
            int Hrestroom = UnityEngine.Random.Range(1, 3);
            string Hlocation = realestate_location[UnityEngine.Random.Range(0, 4)];
            int Hprice = Convert.ToInt32(Hsize / 300.0f * 700000) + Hbedroom * 100000 + Hrestroom * 20000;
            Hprice -= Hprice % 10000;
            House_Detail[i] = new House()
            {
                size = Hsize,
                bedroom = Hbedroom,
                restroom = Hrestroom,
                location = Hlocation,
                price = Hprice,
            };
        }
    }
    public void Load_House_Detail()
    {
        for (int i = 0; i < 5; i++)
        {
            string show = "  ";
            show += House_Detail[i].location + ", ";
            show += Word(House_Detail[i].bedroom, "bedroom") + " ";
            show += Word(House_Detail[i].restroom, "restroom") + " ";
            show += Word(House_Detail[i].size, "square meter") + " ";
            show += "\nPrice : " + string.Format(CultureInfo.InvariantCulture, "{0:N0}", House_Detail[i].price);
            Debug.Log(show);
            House_List_UI[i].GetComponent<Text>().text = show;
            // House_List_UI[i].text = show;
        }
    }
    public void Generate_Condo_Detail()
    {
        int[] location_price = { 400000, 30000, 50000, 50000 };
        for (int i = 0; i < 5; i++)
        {
            int Hsize = UnityEngine.Random.Range(4, 7) * 5;
            int Hbedroom = UnityEngine.Random.Range(1, 3);
            int loactionRandom = UnityEngine.Random.Range(0, 4);
            string Hlocation = realestate_location[loactionRandom];
            int Hprice = Convert.ToInt32(Hsize / 25.0f * 500000) + Hbedroom * 100000 + location_price[loactionRandom];
            Hprice -= Hprice % 10000;
            Condo_Detail[i] = new Condo()
            {
                size = Hsize,
                bedroom = Hbedroom,
                location = Hlocation,
                price = Hprice,
            };
        }
    }
    public void Load_Condo_Detail()
    {
        for (int i = 0; i < 5; i++)
        {
            string show = "  ";
            show += Condo_Detail[i].location + ", ";
            show += Word(Condo_Detail[i].bedroom, "bedroom") + " ";
            show += Word(Condo_Detail[i].size, "square meter") + " ";
            show += "\nPrice : " + string.Format(CultureInfo.InvariantCulture, "{0:N0}", Condo_Detail[i].price);
            Debug.Log(show);
            Condo_List_UI[i].GetComponent<Text>().text = show;
            // House_List_UI[i].text = show;
        }
    }
    public void Generate_Warehouse_Detail()
    {
        int[] location_price = { 300000, 60000, 50000, 50000 };
        for (int i = 0; i < 5; i++)
        {
            int Hsize = UnityEngine.Random.Range(100, 1000);
            int loactionRandom = UnityEngine.Random.Range(0, 4);
            int Hunits = UnityEngine.Random.Range(3, 6) * 10;
            string Hlocation = realestate_location[loactionRandom];
            int Hprice = Convert.ToInt32(Hsize / 100 * 200000) + location_price[loactionRandom];
            Hprice -= Hprice % 10000;
            Warehouse_Detail[i] = new Warehouse()
            {
                size = Hsize,
                units = Hunits,
                location = Hlocation,
                price = Hprice,
            };
        }
    }
    public void Load_Warehouse_Detail()
    {
        for (int i = 0; i < 5; i++)
        {
            string show = "  ";
            show += Warehouse_Detail[i].location + ", ";
            show += Word(Warehouse_Detail[i].size, "square meter") + " ";
            show += Word(Warehouse_Detail[i].units, "Unit") + ", ";
            show += "\nPrice : " + string.Format(CultureInfo.InvariantCulture, "{0:N0}", Warehouse_Detail[i].price);
            Debug.Log(show);
            Warehouse_List_UI[i].GetComponent<Text>().text = show;
            // House_List_UI[i].text = show;
        }
    }
    public string Word(int number, string s)
    {
        if (number == 0)
        {
            return "No " + s;
        }
        if (number == 1)
        {
            return number.ToString() + " " + s;
        }
        return number.ToString() + " " + s + "s";
    }
    public void House_Click(int number)
    {
        Real_Estate_Buy_Menu.SetActive(true);
        selected_house = number;
        realestatebuyMode = 1;
    }
    public void Condo_Click(int number)
    {
        Real_Estate_Buy_Menu.SetActive(true);
        selected_condo = number;
        realestatebuyMode = 2;
    }
    public void Warehouse_Click(int number)
    {
        Real_Estate_Buy_Menu.SetActive(true);
        selected_warehouse = number;
        realestatebuyMode = 3;
    }
    public void Close_House_Menu()
    {
        Real_Estate_Buy_Menu.SetActive(false);
    }
    public void RealEstateBuyBT()
    {
        if (realestatebuyMode == 1)
        {
            Buy_house();
        }
        else if (realestatebuyMode == 2)
        {
            Buy_Condo();
        }
        else if (realestatebuyMode == 3)
        {
            Buy_warehouse();
        }
    }
    public void Buy_house()
    {
        Players_Data[Namelist[index]].house += 1;
        Players_Data[Namelist[index]].Money -= House_Detail[selected_house].price;
        Icon_House_Rendering();
        Debug.Log("House Buy");
    }
    public void Buy_Condo()
    {
        Players_Data[Namelist[index]].condo += 1;
        Players_Data[Namelist[index]].Money -= Condo_Detail[selected_condo].price;
        Icon_Condo_Rendering();
        Debug.Log("Condo Buy");
    }
    public void Buy_warehouse()
    {
        Players_Data[Namelist[index]].warehouse += 1;
        Players_Data[Namelist[index]].Money -= Warehouse_Detail[selected_warehouse].price;
        Icon_Warehouse_Rendering();
        Debug.Log("Warehouse Buy");
    }
    public void Icon_House_Rendering()
    {
        // House_Icon.transform.SetParent(House_Icon_Grid).transform);
        Instantiate(House_Icon, House_Icon_Grid.transform);
    }
    public void Icon_Condo_Rendering()
    {
        // House_Icon.transform.SetParent(House_Icon_Grid).transform);
        Instantiate(Condo_Icon, Condo_Icon_Grid.transform);
    }
    public void Icon_Warehouse_Rendering()
    {
        // House_Icon.transform.SetParent(House_Icon_Grid).transform);
        Instantiate(Warehouse_Icon, Warehouse_Icon_Grid.transform);
    }
    public void OpenBank_Menu(bool open)
    {
        Bank_Menu.SetActive(open);
    }
    public void CalculateInterset()
    {
        int borrowmoney = int.Parse(Bank_Borrow_Money_Input.text);
        double monthlyInterestRate = 6 / 12.0 / 100.0;
        int numberOfPayments = 20 * 12;
        double monthlyPayment = borrowmoney * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, numberOfPayments) /
                                (Math.Pow(1 + monthlyInterestRate, numberOfPayments) - 1);
        // Debug.Log(monthlyPayment);
        monthlyPayment = Convert.ToInt32(monthlyPayment);
        // Debug.Log(borrowmoney);
        // int interestperMonth = Convert.ToInt32(borrowmoney * 0.06f / 12);
        Bank_Monthly_Pay.GetComponent<TMP_Text>().text = "= " + monthlyPayment.ToString() + "/month";
    }
    public void Bank_Borrow()
    {
        Loan x = new()
        {
            amount = int.Parse(Bank_Borrow_Money_Input.text),
            interest = 6,
            LoanTermYears = 20,
        };
        Players_Data[Namelist[index]].Add_Player_Loan(x);
    }
}







public class Stock
{
    public string name;
    public float price;
    public int amount;
    public float bit;
}
public class Warehouse
{
    public int size;
    public int units;
    public int price;
    public string location;
}
public class Condo
{
    public int size;
    public int bedroom;
    public int price;
    public string location;
}
public class House
{
    public int size;
    public int bedroom;
    public int restroom;
    public string location;
    public int price;
}
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
public class PlayerMoney
{
    public int house = 0;
    public List<House> Player_House = new List<House>();
    public List<Condo> Player_Condo = new List<Condo>();
    public List<Warehouse> Player_Warehouse = new List<Warehouse>();
    public List<Loan> Player_Loan = new List<Loan>();
    public List<Stock> Player_Stock = new List<Stock>();
    public int condo = 0;
    public int warehouse = 0;
    public int Net_Worth = 0;
    public int Money = 0;
    public int Salary = 30000;
    public string name = "";
    // public Dictionary<string, Dictionary<float, int>> Player_Stock = new Dictionary<string, Dictionary<float, int>>();

    public void Turn()
    {
        Money += Salary;
    }
    public void Buy_Stock(string name, int amount, float price)
    {
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

        // foreach (var stock in Player_Stock)
        // {
        //     foreach (var x in stock)
        //     {
        //         Debug.Log(x);
        //     }
        // }
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
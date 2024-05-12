// using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class logic3 : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject realestate_menu;
    public GameObject realestate_closebutton;
    public GameObject stockmarket_menu;
    public GameObject stockmarket_closebutton;
    public GameObject TimerBar;
    public GameObject Buy_Stock_Menu;
    public GameObject Input_Amount;
    public GameObject Buy_Stock_Bt;
    public GameObject Sale_Stock_Button;
    public GameObject Close_Stock_Menu_Button;
    public GameObject Random_Event;
    public GameObject Random_Event_Message;
    public GameObject Skip_Button;
    public GameObject[] Arrayplayer = new GameObject[4];
    public GameObject[] Stock_List_UI = new GameObject[5];
    public GameObject[] Player_name_UI = new GameObject[4];
    public GameObject[] House_List_UI = new GameObject[5];
    public TMP_Text ScoreBoard_UI;
    public TMP_Text Portfolio;
    public TMP_Text Player_Balance;
    Dictionary<string, PlayerMoney> Players = new Dictionary<string, PlayerMoney>();
    Dictionary<string, Stock> Stocks = new Dictionary<string, Stock>();
    House[] House_Detail = new House[5];
    Vector2[] BoardPosition = new Vector2[4];
    Logic2 SelectMenuLogic;
    int index = -1;
    int NumberPlayer;
    int RoundCount = 0;
    bool[] PlayerDetail = new bool[4];
    bool IsTimer = true;
    string[] Namelist = { "P1", "P2", "P3", "P4" };
    string[] StockList = { "CPALL", "GULF", "PTT", "ADVANC", "BTS" };
    string[] eventmessage = { "Right now, the economy is getting better", "Now, the economy is going down." };
    string[] realestate_location = {"Bangkok","Saputprakran","Rayong","ChingMai"};
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
                Players[Namelist[i]] = new PlayerMoney
                {
                    name = Namelist[i],
                    Salary = 100000,

                };
                Players[Namelist[i]].Turn();
            }
        }

        for (int i = 0; i < StockList.Length; i++)
        {
            Stocks[StockList[i]] = new Stock()
            {
                name = StockList[i],
                price = StockPrice[i],
                bit = StockBit[i],
            };
        }
        for(int i =0;i<5;i++){
            House_Detail[i] = new House(){
                size = UnityEngine.Random.Range(200,300);
                bedroom = UnityEngine.Random.Range(2,5);
                restroom = UnityEngine.Random.Range(1,3);
                location = realestate_location[UnityEngine.Random.Range(0,4)];
            }
        }
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
            Load_player_Portolio(index);
            Update_Player_Balance();
            RoundCount++;
        }
        //generate price;
        if (NumberPlayer == RoundCount)
        {
            Give_Player_Salary();
            Update_Net_worth();
            Update_ScoreBoard();
            Random_Event_Logic();
            foreach (var stock in Stocks)
            {
                stock.Value.price = Generate_Stock_Price(stock.Value.price, stock.Value.bit);
            }
            for (int i = 0; i < StockPrice.Length; i++)
            {
                Stock_List_UI[i].GetComponent<TMP_Text>().text = StockList[i] + "  " + Stocks[StockList[i]].price.ToString("F2");
                // Debug.Log(StockList[i] + "  " + StockPrice[i].ToString("F2"));
            }
            RoundCount = 0;

        }
        Timer_UI(Timer, TimeEachTurn, TimerBar);
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
        TimerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(686, 17);
    }
    public void OpenRealEstate()
    {
        ToggleButton(realestate_closebutton, realestate_menu, true);
    }
    public void CloseRealEstate()
    {
        ToggleButton(realestate_closebutton, realestate_menu, false);
    }
    public void OpenStockMarket()
    {
        ToggleButton(stockmarket_closebutton, stockmarket_menu, true);
    }
    public void CloseStockMarket()
    {
        ToggleButton(stockmarket_closebutton, stockmarket_menu, false);
        Buy_Stock_Menu.SetActive(false);
    }
    public void Open_Stock_Menu_Logic(string name)
    {
        Buy_Stock_Menu.SetActive(true);
        Selected_Stock_Name = name;
    }
    public void Close_Stock_Menu_Logic()
    {
        Buy_Stock_Menu.SetActive(false);
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
        Players[Namelist[index]].Buy_Stock(Selected_Stock_Name, int.Parse(Input_Amount.GetComponent<InputField>().text), Stocks[Selected_Stock_Name].price);
        Load_player_Portolio(index);
        Update_ScoreBoard();
        Update_Player_Balance();
    }
    public void Load_player_Portolio(int index)
    {
        string port = Players[Namelist[index]].Show_Player_Stock();
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
        var SortedMoney = Players.OrderByDescending(pair => pair.Value.Net_Worth);
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
    public void Random_Event_Logic()
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
        Random_Event.SetActive(true);
        Random_Event_Message.GetComponent<TMP_Text>().text = eventmessage[random_message];
    }
    public void Close_Random_Event()
    {
        TimerUpdate(true);
        Random_Event.SetActive(false);
    }
    public void Update_Net_worth()
    {
        foreach (var player in Players)
        {
            player.Value.Calculate_Net_Worth(Stocks);
        }
    }
    public void Give_Player_Salary()
    {
        foreach (var player in Players)
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
        Player_Balance.text = "Balance : " + Players[Namelist[index]].Money.ToString();
    }
}







public class Stock
{
    public string name;
    public float price;
    public int amount;
    public float bit;
}
public class House{
    public int size;
    public int bedroom;
    public int restroom;
    public string location;
}
public class PlayerMoney
{

    public int Net_Worth = 0;
    public int Money = 0;
    public int Salary = 30000;
    public string name = "";
    // public Dictionary<string, Dictionary<float, int>> Player_Stock = new Dictionary<string, Dictionary<float, int>>();
    public List<Stock> Player_Stock = new List<Stock>();
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
    public void Calculate_Net_Worth(Dictionary<string, Stock> Stocks)
    {
        int worth = Money;
        foreach (var stock in Player_Stock)
        {
            worth += Convert.ToInt32(Stocks[stock.name].price * stock.amount);
        }
        Net_Worth = worth;
    }
}
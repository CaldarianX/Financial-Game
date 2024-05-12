using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Numerics;
using System;
public class Logic2 : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pic1;
    public GameObject pic2;
    public GameObject pic3;
    public GameObject pic4;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public bool p1Active = false;
    public bool p2Active = false;
    public bool p3Active = false;
    public bool p4Active = false;
    public int PlayerNumber = 0;

    private bool debug = false;
    public void Click1()
    {
        ShowPlayer(pic1, player1, ref p1Active);
        // Debug.Log("1");
    }
    public void Click2()
    {
        ShowPlayer(pic2, player2, ref p2Active);
        // Debug.Log("2");
    }
    public void Click3()
    {
        ShowPlayer(pic3, player3, ref p3Active);
        // Debug.Log("3");
    }
    public void Click4()
    {
        ShowPlayer(pic4, player4, ref p4Active);
        // Debug.Log("4");
    }

    public void Update()
    {
        if (debug)
        {
            Debug.Log("___" + PlayerNumber.ToString());
        }

    }

    public void ShowPlayer(GameObject player, GameObject pic, ref bool active)
    {
        ShowPic(pic);
        active = !active;
        ToggleSetactive(player);
    }
    public void ShowPic(GameObject pic)
    {
        Image img = pic.GetComponent<Image>();
        if (img.color.a == 0f)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            // Debug.Log("Now showing");
            return;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
    }
    public void ToggleSetactive(GameObject name)
    {
        if (name.activeInHierarchy)
        {
            name.SetActive(false);
            PlayerNumber--;
            return;
        }
        name.SetActive(true);
        PlayerNumber++;
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void StartGame()
    {
        // Debug.Log("-----------------------------------   " + PlayerNumber.ToString());
        // if (PlayerNumber != 0)
        // {
        SceneManager.LoadScene(2);
        // }
    }
    public bool[] PlayerDetail()
    {
        bool[] playerDetails = new bool[4];
        playerDetails[0] = p1Active;
        playerDetails[1] = p2Active;
        playerDetails[2] = p3Active;
        playerDetails[3] = p4Active;
        return playerDetails;
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}


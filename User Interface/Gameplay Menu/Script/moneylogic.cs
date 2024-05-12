using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneylogic : MonoBehaviour
{

}


public class Player : MonoBehaviour
{
    int Money = 0;
    int Salary = 30000;

    public void Turn()
    {
        Money += Salary;
    }
}
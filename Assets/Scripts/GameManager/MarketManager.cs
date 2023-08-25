using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager instance;

    int totalMoney;

    void Awake()
    {
        instance = this;   
    }

    public void PlusTotalMoney(int money)
    {
        totalMoney += money;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager instance;


    public int customerCount { get; set; }
    public int customerTotalCycleTime { get; set; }
    public int deadCustomer { get; set; }
    public int totalMoney { get; set; }



    void Awake()
    {
        instance = this;   
    }

    public int customerCycleTime
    {
        get 
        {
            if (deadCustomer == 0)  return 0;
            return customerTotalCycleTime / deadCustomer; 
        }
    }
}

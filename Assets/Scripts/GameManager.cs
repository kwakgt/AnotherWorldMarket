using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Unit selectedUnit;

    void Awake()
    {
        instance = this;
    }
}

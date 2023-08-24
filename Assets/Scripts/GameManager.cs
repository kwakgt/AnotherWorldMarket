using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Unit selectedUnit;
    public GameMode gameMode = GameMode.Seller;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        Debug.Log(gameMode);
        Pause();
    }

    void Pause()
    {
        if(InputManager.instance.bKeyDown)
        {
            ChangeBuliderMode();
            InputManager.instance.bKeyDown = false;
        }

        if (gameMode == GameMode.Builder)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    
    public void ChangeBuliderMode()
    {
        if(gameMode != GameMode.Builder) { gameMode = GameMode.Builder; }
        else { gameMode = GameMode.Seller; }
    }   
    public enum GameMode { Seller, Builder }
}

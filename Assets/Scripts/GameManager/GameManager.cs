using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameMode gameMode { get; private set; } = GameMode.Seller;
    
    public Unit selectedUnit;
    public Shelf selectedShelf;
    public Warehouse selectedWarehouse;


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        ChangeMode();
        PlayAndPause();
    }

    void PlayAndPause()
    {
        if(InputManager.instance.spaceKeyDown)
        {
            if (Time.timeScale != 1f)   Time.timeScale = 1f;
            else                        Time.timeScale = 0f;

            InputManager.instance.spaceKeyDown = false;
        }
    }

    void ChangeMode()
    {
        if(InputManager.instance.bKeyDown)
        {
            if (gameMode != GameMode.Builder)   ChangeBuliderMode();
            else                                ChangeSellerMode();

            if (gameMode == GameMode.Seller) Time.timeScale = 1f;   //모드가 판매모드이면 정상속도로 변경
            InputManager.instance.bKeyDown = false;
        }

        if (gameMode == GameMode.Builder) Time.timeScale = 0;   //건설모드이면 무조건 일시정지
    }

    public void ChangeBuliderMode()   //메뉴버튼 사용
    {
        gameMode = GameMode.Builder;
    }

    public void ChangeSellerMode()    //메뉴버튼 사용
    {
        gameMode = GameMode.Seller;
    }

    public void ChangeGameSpeed(int controlKey = 3)   //스피드버튼 사용
    {
        if (GameManager.instance.gameMode == GameManager.GameMode.Builder) return;

        switch (controlKey)
        {
            case 1: { Time.timeScale = 0.5f; break; }
            case 2: { Time.timeScale = 0; break; }
            case 3: { Time.timeScale = 1f; break; }
            case 4: { Time.timeScale = 2f; break; }
            default: { Time.timeScale = 1f; break; }
        }
    }
    public enum GameMode { Seller, Builder }
}

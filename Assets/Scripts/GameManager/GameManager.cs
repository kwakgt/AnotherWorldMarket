using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Unit selectedUnit;
    public Shelf selectedShelf;
    public Warehouse selectedWarehouse;

    public GameMode gameMode { get; private set; } = GameMode.Seller;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        ChangeMode();
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
    public enum GameMode { Seller, Builder }
}

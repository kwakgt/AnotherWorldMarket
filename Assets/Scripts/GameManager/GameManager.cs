using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    GameMode gameMode = GameMode.Seller;
    
    public Unit selectedUnit;
    public Shelf selectedShelf;
    public Warehouse selectedWarehouse;

    GameObject constructionPanel;
    void Awake()
    {
        instance = this;
        constructionPanel = GameObject.Find("ConstructionPanel");
    }

    void Update()
    {
        ChangeMode_bKeyDown();
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

    void ChangeMode_bKeyDown()
    {
        if(InputManager.instance.bKeyDown)
        {
            InputManager.instance.bKeyDown = false;
            ChangeMode();
        }

        if (gameMode == GameMode.Builder) Time.timeScale = 0;   //건설모드이면 무조건 일시정지
    }

    public void ChangeMode()   //메뉴버튼 사용
    {
        if (gameMode != GameMode.Builder)
        {
            gameMode = GameMode.Builder;
            constructionPanel.SetActive(true);  //건설모드면 건설패널 열기
        }
        else
        {
            gameMode = GameMode.Seller;
            constructionPanel.SetActive(false); //다른모드면 건설패널 닫기
        }

        if (gameMode == GameMode.Seller) Time.timeScale = 1f;   //모드가 판매모드이면 정상속도로 변경
    }

    public void ChangeGameSpeed(int controlKey = 3)   //스피드버튼 사용
    {
        if (instance.gameMode == GameMode.Builder) return;

        switch (controlKey)
        {
            case 1: { Time.timeScale = 0.5f; break; }
            case 2: { Time.timeScale = 0; break; }
            case 3: { Time.timeScale = 1f; break; }
            case 4: { Time.timeScale = 2f; break; }
            default: { Time.timeScale = 1f; break; }
        }
    }

    public bool CompareTo(GameMode _gameMode)
    {
        if(gameMode == _gameMode)
            return true;
        else
            return false;
    }
    public enum GameMode { Seller, Builder }
}

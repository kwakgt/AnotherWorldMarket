using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Structure[] portals;

    public Unit selectedUnit;
    public Shelf selectedShelf;
    public Warehouse selectedWarehouse;

    GameObject constructionPanel;           //활성화/비활성화
    GameMode gameMode = GameMode.Seller;    //현재 게임모드
    float preTimeScale = 1f;                 //이전 속도


    public int day { get; set; } = 1;           //현재일수
    public int hour { get; set; }           //현재시간
    void Awake()
    {
        instance = this;
        portals = transform.GetChild(0).GetComponentsInChildren<Structure>();
        constructionPanel = GameObject.Find("ConstructionPanel");
    }

    void Update()
    {
        ChangeMode_bKeyDown();
        PlayAndPause();
    }

    void PlayAndPause()
    {
        if (InputManager.instance.spaceKeyDown)
        {
            InputManager.instance.spaceKeyDown = false;
            if (Time.timeScale > 0)
            {
                ChangeGameSpeed(0);
            }
            else
            {
                ChangeGameSpeed(preTimeScale);
            }
        }
    }

    void ChangeMode_bKeyDown()
    {
        if (InputManager.instance.bKeyDown)
        {
            InputManager.instance.bKeyDown = false;
            ChangeMode();
        }
    }

    public void ChangeMode()   //메뉴버튼 사용
    {
        if (gameMode != GameMode.Builder)
        {
            gameMode = GameMode.Builder;
            ChangeGameSpeed(0);
            constructionPanel.SetActive(true);
        }
        else
        {
            gameMode = GameMode.Seller;
            ChangeGameSpeed(preTimeScale);
            constructionPanel.SetActive(false);
        }
    }

    public void ChangeGameSpeed(float timeScale = 1)   //스피드버튼 사용
    {
        if (Time.timeScale != 0)
            preTimeScale = Time.timeScale;  //preTimeScale에 0값 제외

        if (instance.gameMode == GameMode.Builder)  //건설모드 시 속도는 무조건 0
        {
            Time.timeScale = 0f; 
            return;
        }

        switch (timeScale)
        {
            case 0: { Time.timeScale = 0f; break; }
            case 1: { Time.timeScale = 1f; break; }
            case 2: { Time.timeScale = 2f; break; }
            case 3: { Time.timeScale = 3f; break; }
            default: { Time.timeScale = 1f; break; }
        }
    }

    public bool Equals(GameMode other)
    {
        if (gameMode == other)
            return true;
        else
            return false;
    }


    public enum GameMode { Seller, Builder }
}

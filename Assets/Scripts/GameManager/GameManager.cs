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

            if (gameMode == GameMode.Seller) Time.timeScale = 1f;   //��尡 �ǸŸ���̸� ����ӵ��� ����
            InputManager.instance.bKeyDown = false;
        }

        if (gameMode == GameMode.Builder) Time.timeScale = 0;   //�Ǽ�����̸� ������ �Ͻ�����
    }

  
    public void ChangeBuliderMode()   //�޴���ư ���
    {
        gameMode = GameMode.Builder;
    }

    public void ChangeSellerMode()    //�޴���ư ���
    {
        gameMode = GameMode.Seller;
    }
    public enum GameMode { Seller, Builder }
}
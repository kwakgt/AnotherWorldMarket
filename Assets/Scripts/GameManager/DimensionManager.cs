using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;
using System;
using Random = UnityEngine.Random;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager instance;

    //차원에 있는 직원 리스트
    public Dictionary<Dimension, List<Staff>> dimensionStaff { get; private set; }
    //차원에서 얻을 수 있는 자원(작업타입에 따른 분류 저장).  ex) Hungting : 빅터고기,빅터가죽. Fishing : 피라사바 
    Dictionary<Dimension, Dictionary<StaffWork, List<Item>>> items;

    void Awake()
    {
        instance = this;

        //차원의 직원리스트 초기화
        dimensionStaff = new Dictionary<Dimension, List<Staff>>();
        foreach(Dimension name in Enum.GetValues(typeof(Dimension)))
        {
            dimensionStaff.Add(name, new List<Staff>());
        }

        //차원의 자원리스트 초기화
        items = new Dictionary<Dimension, Dictionary<StaffWork, List<Item>>>();
        foreach(Dimension name in Enum.GetValues(typeof(Dimension)))
        {
            items.Add(name, new Dictionary<StaffWork, List<Item>>());
            
            foreach(StaffWork command in Enum.GetValues(typeof(StaffWork)))
            {
                if (command == StaffWork.Emptying || command == StaffWork.Checking || command == StaffWork.Finding || command == StaffWork.Carrying || command == StaffWork.Teleporting) //불필요한 명령 제거
                    continue;

                items[name].Add(command, new List<Item>());
            }
        }

        //자원리스트에 아이템 저장
        
    }

    void Start()
    {
        InitDimentionItem();
    }

    void InitDimentionItem()
    {
        for(int i = 0; i < ItemManager.instance.CountOfAllItem(); i++)
        {
            Item item = ItemManager.instance.GetItem(i, 1000);
            items[item.dimension][item.workType].Add(item);
        }


        //Astaria
        //{
        //    //Felling
        //    items[Dimension.Astaria][WorkType.Felling].Add(ItemManager.instance.GetItem("통나무", 1000));
        //    //Mining
        //    items[Dimension.Astaria][WorkType.Mining].Add(ItemManager.instance.GetItem("광석", 1000));
        //    //Collecting
        //    items[Dimension.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("초사과", 1000));
        //    items[Dimension.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("생강무", 1000));
        //    //Hunting
        //    items[Dimension.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("빅버고기", 1000));
        //    items[Dimension.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("빅버가죽", 1000));
        //    //Fishing
        //    items[Dimension.Astaria][WorkType.Fishing].Add(ItemManager.instance.GetItem("피라사바", 1000));

        //    //TODO::차원 아이템 추가
        //}

        //TODO:: 차원 추가
    }



    public void EnterDimension(Dimension enter, Staff staff)
    {
        dimensionStaff[enter].Add(staff);
    }

    public void ShiftDimension(Dimension from, Dimension to, Staff staff)
    {
        dimensionStaff[from].Remove(staff);
        dimensionStaff[to].Add(staff);
    }


    public Item GetItem(Dimension dimention, StaffWork workType)
    {
        List<Item> list = items[dimention][workType];
        if (list == null || list.Count <= 0) return null;   //아이템 목록이 없으면 NULL;
        
        return list[Random.Range(0, list.Count)];   //TODO:: 리스트에 있는 아이템마다 채집확률설정
    }
}

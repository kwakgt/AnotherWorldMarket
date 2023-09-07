using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;
using System;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager instance;

    //차원에 있는 직원 리스트
    Dictionary<Dimention, List<Staff>> dimention;
    //차원에서 얻을 수 있는 자원(작업타입에 따른 분류 저장).  ex) Hungting : 빅터고기,빅터가죽. Fishing : 피라사바 
    Dictionary<Dimention, Dictionary<WorkType, List<Item>>> items;

    void Awake()
    {
        instance = this;

        //차원의 직원리스트 초기화
        dimention = new Dictionary<Dimention, List<Staff>>();
        foreach(Dimention name in Enum.GetValues(typeof(Dimention)))
        {
            dimention.Add(name, new List<Staff>());
        }

        //차원의 자원리스트 초기화
        items = new Dictionary<Dimention, Dictionary<WorkType, List<Item>>>();
        foreach(Dimention name in Enum.GetValues(typeof(Dimention)))
        {
            items.Add(name, new Dictionary<WorkType, List<Item>>());
            
            foreach(WorkType command in Enum.GetValues(typeof(WorkType)))
            {
                if (command == WorkType.Emptying || command == WorkType.Checking || command == WorkType.Finding || command == WorkType.Carrying || command == WorkType.Teleporting) //불필요한 명령 제거
                    continue;

                items[name].Add(command, new List<Item>());
            }
        }

        //자원리스트에 아이템 저장
        InitDimentionItem();
    }

    void InitDimentionItem()
    {
        //Astaria
        {
            //Felling
            items[Dimention.Astaria][WorkType.Felling].Add(ItemManager.instance.GetItem("통나무", 1000));
            //Mining
            items[Dimention.Astaria][WorkType.Mining].Add(ItemManager.instance.GetItem("광석", 1000));
            //Collecting
            items[Dimention.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("초사과", 1000));
            items[Dimention.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("생강무", 1000));
            //Hunting
            items[Dimention.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("빅버고기", 1000));
            items[Dimention.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("빅버가죽", 1000));
            //Fishing
            items[Dimention.Astaria][WorkType.Fishing].Add(ItemManager.instance.GetItem("파라사바", 1000));
        }
    }



    void EnterDimention(Dimention enter, Staff staff)
    {
        dimention[enter].Add(staff);
    }

    void ExitDimention(Dimention exit, Staff staff)
    {
        dimention[exit].Remove(staff);
    }

    void ShiftDimention(Dimention from, Dimention to, Staff staff)
    {
        dimention[from].Remove(staff);
        dimention[to].Add(staff);
    }
}

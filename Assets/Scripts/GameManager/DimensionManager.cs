using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;
using System;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager instance;

    //������ �ִ� ���� ����Ʈ
    Dictionary<Dimention, List<Staff>> dimention;
    //�������� ���� �� �ִ� �ڿ�(�۾�Ÿ�Կ� ���� �з� ����).  ex) Hungting : ���Ͱ���,���Ͱ���. Fishing : �Ƕ��� 
    Dictionary<Dimention, Dictionary<WorkType, List<Item>>> items;

    void Awake()
    {
        instance = this;

        //������ ��������Ʈ �ʱ�ȭ
        dimention = new Dictionary<Dimention, List<Staff>>();
        foreach(Dimention name in Enum.GetValues(typeof(Dimention)))
        {
            dimention.Add(name, new List<Staff>());
        }

        //������ �ڿ�����Ʈ �ʱ�ȭ
        items = new Dictionary<Dimention, Dictionary<WorkType, List<Item>>>();
        foreach(Dimention name in Enum.GetValues(typeof(Dimention)))
        {
            items.Add(name, new Dictionary<WorkType, List<Item>>());
            
            foreach(WorkType command in Enum.GetValues(typeof(WorkType)))
            {
                if (command == WorkType.Emptying || command == WorkType.Checking || command == WorkType.Finding || command == WorkType.Carrying || command == WorkType.Teleporting) //���ʿ��� ���� ����
                    continue;

                items[name].Add(command, new List<Item>());
            }
        }

        //�ڿ�����Ʈ�� ������ ����
        InitDimentionItem();
    }

    void InitDimentionItem()
    {
        //Astaria
        {
            //Felling
            items[Dimention.Astaria][WorkType.Felling].Add(ItemManager.instance.GetItem("�볪��", 1000));
            //Mining
            items[Dimention.Astaria][WorkType.Mining].Add(ItemManager.instance.GetItem("����", 1000));
            //Collecting
            items[Dimention.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("�ʻ��", 1000));
            items[Dimention.Astaria][WorkType.Collecting].Add(ItemManager.instance.GetItem("������", 1000));
            //Hunting
            items[Dimention.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("�������", 1000));
            items[Dimention.Astaria][WorkType.Hunting].Add(ItemManager.instance.GetItem("�������", 1000));
            //Fishing
            items[Dimention.Astaria][WorkType.Fishing].Add(ItemManager.instance.GetItem("�Ķ���", 1000));
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
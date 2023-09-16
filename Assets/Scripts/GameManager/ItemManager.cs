using EnumManager;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    //Item DB
    Dictionary<string, Item> itemNameDB = new Dictionary<string, Item>();
    Dictionary<int, Item> itemUniqueKeyDB = new Dictionary<int, Item>();

    void Awake()
    {
        instance = this;
        ReadItemDBToCSV();
    }

    void ReadItemDBToCSV()
    {
        var list = CSVReader.Read("Document/AnotherWorldMarketItemTextAsset");

        for(int i = 0; i < list.Count; i++)
        {
            string spritePath = "Sprite/Item/" + list[i]["nameEN"];
            Sprite sprite = Resources.Load<Sprite>(spritePath); //리소스에서 Sprite 불러오기
            Dimension dimension = (Dimension)Enum.Parse(typeof(Dimension), (string)list[i]["dimension"]);  //Dimension enum으로 변경
            WorkType work = (WorkType)Enum.Parse(typeof(WorkType), (string)list[i]["workType"]);              //WorkType enum으로 변경
            itemNameDB.Add((string)list[i]["name"], 
                new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], dimension, work, (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
            itemUniqueKeyDB.Add((int)list[i]["uniqueKey"], 
                new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], dimension, work, (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
        }
    }

    public Item GetItem(string name, int amount = 0)
    {
        if (itemNameDB.ContainsKey(name))
        {
            Item item = new Item(itemNameDB[name]);
            item.PlusAmount(amount);
            return item;
        }
        else return null;
    }

    public Item GetItem(int uniqueKey, int amount = 0)
    {
        if (itemUniqueKeyDB.ContainsKey(uniqueKey))
        {
            Item item = new Item(itemUniqueKeyDB[uniqueKey]);
            item.PlusAmount(amount);
            return item;
        }
        else return null;
    }

    public Item GetRandomItem()
    {
        Item item;
        while (true)
        {
            if (itemUniqueKeyDB.TryGetValue(Random.Range(0, itemUniqueKeyDB.Count), out item))
            {
                if (item.amountOfShelf == 0)
                    continue;
                return new Item(item);
            }
            else
                return null;
        }
    }

    public int CountOfAllItem()     //아이템 총 개수
    {
        return itemUniqueKeyDB.Count;
    }
}

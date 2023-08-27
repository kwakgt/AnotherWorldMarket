using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    Dictionary<string, Item> itemNameDB = new Dictionary<string, Item>();
    Dictionary<int, Item> itemUniqueKeyDB = new Dictionary<int, Item>();

    void Awake()
    {
        instance = this;
        ReadItemDBToCSV();
    }

    void ReadItemDBToCSV()
    {
        var list = new List<Dictionary<string, object>>();
        list = CSVReader.Read("AnotherWorldMarketItemTextAsset");

        for(int i = 0; i < list.Count; i++)
        {
            string spritePath = "Sprite/" + list[i]["name"];
            Sprite sprite = Resources.Load<Sprite>(spritePath); //리소스에서 Sprite 불러오기
            itemNameDB.Add((string)list[i]["name"], new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
            itemUniqueKeyDB.Add((int)list[i]["uniqueKey"], new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
        }
    }

    public Item GetItem(string name)
    {
        if (itemNameDB.ContainsKey(name))
        {
            Item item = new Item(itemNameDB[name]);
            return item;
        }
        else return null;
    }

    public Item GetItem(int uniqueKey)
    {
        if (itemUniqueKeyDB.ContainsKey(uniqueKey))
        {
            Item item = new Item(itemUniqueKeyDB[uniqueKey]);
            return item;
        }
        else return null;
    }

    public Item GetRandomItem()
    {
        Item item;
        if (itemUniqueKeyDB.TryGetValue(Random.Range(0, itemUniqueKeyDB.Count), out item))
            return new Item(item);
        else
            return null;
    }

    public int CountOfAllItem()     //아이템 총 개수
    {
        return itemUniqueKeyDB.Count;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            Sprite sprite = Resources.Load<Sprite>(spritePath); //���ҽ����� Sprite �ҷ�����
            itemNameDB.Add((string)list[i]["name"], new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], (int)list[i]["price"], sprite));
            itemUniqueKeyDB.Add((int)list[i]["uniqueKey"], new Item((string)list[i]["name"], (int)list[i]["uniqueKey"], (int)list[i]["price"], sprite));
        }
    }

    public Item GetItem(string name)
    {
        Item item = new Item(itemNameDB[name]);
        if (itemNameDB.ContainsKey(name))   return item;
        else                            return null;
    }

    public Item GetRandomItem()
    {
        Item item;
        if (itemUniqueKeyDB.TryGetValue(Random.Range(0, itemUniqueKeyDB.Count), out item))
            return new Item(item);
        else
            return null;
    }
}
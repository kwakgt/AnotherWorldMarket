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

    //RecipeDB - ItemDB를 참조하기 때문에 여기에 만듬
    Dictionary<StaffWork, List<Recipe>> recipeDB = new Dictionary<StaffWork, List<Recipe>>();

    void Awake()
    {
        instance = this;
        ReadItemDBToCSV();
        ReadRecipeDBToCSV();
    }

    //ReadCSV   ==============================================================================================================================================================================
    void ReadItemDBToCSV()
    {
        var list = CSVReader.Read("Document/AnotherWorldMarketItemTextAsset");

        for(int i = 0; i < list.Count; i++)
        {
            string spritePath = "Sprite/Item/" + list[i]["nameEN"];
            Sprite sprite = Resources.Load<Sprite>(spritePath); //리소스에서 Sprite 불러오기
            Dimension dimension = (Dimension)Enum.Parse(typeof(Dimension), (string)list[i]["dimension"]);  //Dimension enum으로 변경
            StaffWork work = (StaffWork)Enum.Parse(typeof(StaffWork), (string)list[i]["workType"]);              //WorkType enum으로 변경
            itemNameDB.Add((string)list[i]["nameEN"], 
                new Item((string)list[i]["nameEN"], (int)list[i]["uniqueKey"], dimension, work, (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
            itemUniqueKeyDB.Add((int)list[i]["uniqueKey"], 
                new Item((string)list[i]["nameEN"], (int)list[i]["uniqueKey"], dimension, work, (int)list[i]["price"], (int)list[i]["amountOfShelf"], (int)list[i]["amountOfWarehouse"], sprite));
        }
    }

    void ReadRecipeDBToCSV()
    {
        //CSV 빈칸은 string에서 Null이 아닌 ""으로 저장된다.
        var list = CSVReader.Read("Document/AnotherWorldMarketRecipe");
        recipeDB.Add(StaffWork.Cooking, new List<Recipe>() { new Recipe(StaffWork.Cooking) });
        recipeDB.Add(StaffWork.Cutting, new List<Recipe>() { new Recipe(StaffWork.Cutting) });
        recipeDB.Add(StaffWork.Drying, new List<Recipe>() { new Recipe(StaffWork.Drying) });
        recipeDB.Add(StaffWork.Juicing, new List<Recipe>() { new Recipe(StaffWork.Juicing) });
        recipeDB.Add(StaffWork.Melting, new List<Recipe>() { new Recipe(StaffWork.Melting) });
        recipeDB.Add(StaffWork.Mixing, new List<Recipe>() { new Recipe(StaffWork.Mixing) });
        recipeDB.Add(StaffWork.Packaging, new List<Recipe>() { new Recipe(StaffWork.Packaging) });

        for (int i = 0; i < list.Count; i++)
        {
            //아이템명이 ""이면 Null 리턴
            StaffWork work = (StaffWork)Enum.Parse(typeof(StaffWork), (string)list[i]["workType"]);
            Item product = GetItem((string)list[i]["product"], (int)list[i]["amount"]);
            Item item1 = GetItem((string)list[i]["item1"], (int)list[i]["amount1"]);
            Item item2 = GetItem((string)list[i]["item2"], (int)list[i]["amount2"]);
            Item item3 = GetItem((string)list[i]["item3"], (int)list[i]["amount3"]);
            Item item4 = GetItem((string)list[i]["item4"], (int)list[i]["amount4"]);
            Item item5 = GetItem((string)list[i]["item5"], (int)list[i]["amount5"]);
            recipeDB[work].Add(new Recipe(work, product, item1, item2, item3, item4, item5));
        }
    }

    //Item  ==============================================================================================================================================================================
    public Item GetItem(string name, int amount = 0)
    {
        if (itemNameDB.ContainsKey(name))
        {
            Item item = new Item(itemNameDB[name]);
            item.PlusAmount(amount);
            return item;
        }
        
        return null;
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

    //Recipe    ==============================================================================================================================================================================
    public List<Recipe> GetRecipe(StaffWork work)
    {
        return recipeDB[work];
    }
}

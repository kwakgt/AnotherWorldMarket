using EnumManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance;

    //RecipeDB
    Dictionary<WorkType, List<Recipe>> recipeDB = new Dictionary<WorkType, List<Recipe>>();

    void Awake()
    {
        instance = this;
        ReadItemDBToCSV();
    }



    void ReadItemDBToCSV()
    {
        var list = CSVReader.Read("Document/AnotherWorldMarketRecipe");
        recipeDB.Add(WorkType.Cooking, new List<Recipe>());
        recipeDB.Add(WorkType.Cutting, new List<Recipe>());
        recipeDB.Add(WorkType.Drying, new List<Recipe>());
        recipeDB.Add(WorkType.Juicing, new List<Recipe>());
        recipeDB.Add(WorkType.Melting, new List<Recipe>());
        recipeDB.Add(WorkType.Mixing, new List<Recipe>());
        recipeDB.Add(WorkType.Packaging, new List<Recipe>());

        for (int i = 0; i < list.Count; i++)
        {
            WorkType work = (WorkType)Enum.Parse(typeof(WorkType), (string)list[i]["workType"]);
            Item product = ItemManager.instance.GetItem((string)list[i]["product"], (int)list[i]["amount"]);
            Item item1 = ItemManager.instance.GetItem((string)list[i]["item1"], (int)list[i]["amount1"]);
            Item item2 = ItemManager.instance.GetItem((string)list[i]["item2"], (int)list[i]["amount2"]);
            Item item3 = ItemManager.instance.GetItem((string)list[i]["item3"], (int)list[i]["amount3"]);
            Item item4 = ItemManager.instance.GetItem((string)list[i]["item4"], (int)list[i]["amount4"]);
            Item item5 = ItemManager.instance.GetItem((string)list[i]["item5"], (int)list[i]["amount5"]);


            recipeDB[work].Add(new Recipe(work, product, item1, item2, item3, item4, item5));
        }
    }
}

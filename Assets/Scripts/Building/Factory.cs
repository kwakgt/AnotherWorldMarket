using EnumManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Structure
{
    //작업 타입
    WorkType workType;
    //일하고 있는 직원
    Staff[] staffs = new Staff[3];
    //레시피 리스트
    List<Recipe> recipes = new List<Recipe>();
    //레시피 재료
    Item[,] materials = new Item[3, 5];
    //인벤토리
    List<Item> inventory = new List<Item>();

    protected override void Awake()
    {
        base.Awake();
        recipes = RecipeManager.instance.GetRecipe(workType);
    }
}

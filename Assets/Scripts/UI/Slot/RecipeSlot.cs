using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeSlot : MonoBehaviour
{
    UnitSlot unitSlot;
    TMP_Dropdown recipeList;
    ItemSlot[] items; //10개중 앞에 5개는 레시피, 뒤에 5개는 재료

    int factoryIndex;
    Factory factory;
    Staff staff;
    
    void Awake()
    {
        unitSlot = transform.GetComponentInChildren<UnitSlot>();
        recipeList = transform.GetComponentInChildren<TMP_Dropdown>();
        items = transform.GetComponentsInChildren<ItemSlot>();
    }

    void Update()
    {
        Display();
    }

    public void SetFactory(int factoryIndex, Factory factory)
    {
        this.factoryIndex = factoryIndex;
        this.factory = factory;
        
        //레시피 드롭다운 초기화
        if (recipeList.options.Count <= 0)
        {
            for (int i = 0; i < this.factory.recipes.Count; i++)
            {
                Item product = this.factory.recipes[i].product;
                if (product == null)
                {
                    recipeList.options.Add(new TMP_Dropdown.OptionData("None", null));
                }
                else
                {
                    recipeList.options.Add(new TMP_Dropdown.OptionData(product.name, product.sprite));
                }
            }

            recipeList.captionText.text = recipeList.options[0].text;
            recipeList.captionImage.sprite = recipeList.options[0].image;
        }
    }

    void Display()
    {
        if (factory == null) return;

        //유닛슬롯 초기화
        staff = factory.staffs[factoryIndex];
        unitSlot.SetStaff(staff);

        //레시피와 재료 초기화
        for (int i = 0; i < 5; i++)
        {
            items[i].SetItem(factory.selectedRecipe[factoryIndex].items[i]);
        }
        
        for (int i = 5; i < items.Length; i++)
        {
            items[i].SetItem(factory.materials[factoryIndex][i - 5]);
        }
    }

    public void OnValueChanged()
    {
        //공장 선택된 레시피 변경
        factory.SelectRecipe(factoryIndex, recipeList.value);

        //공장 레시피 아이템 변경
        for (int i = 0; i < 5; i++)
        {
            items[i].SetItem(factory.selectedRecipe[factoryIndex].items[i]);
        }
    }
}

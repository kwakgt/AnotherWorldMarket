using EnumManager;


public class Recipe
{
    public StaffWork workType { get;}
    public Item product { get;}
    public Item[] items { get; } = new Item[5]; 
    
    
    bool[] check = new bool[5];

    public Recipe(StaffWork _workType)
    {
        workType = _workType;
    }

    public Recipe(StaffWork _workType, Item _product, Item item1, Item item2, Item item3, Item item4, Item item5)
    {
        workType = _workType;
        product = _product;
        items[0] = item1;
        items[1] = item2;
        items[2] = item3;
        items[3] = item4;
        items[4] = item5;
    }



    public bool CompareMaterial(Item[] others)
    {
        if (ReferenceEquals(others, null)) return false;
        if (ReferenceEquals(product, null)) return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)                                                       //레시피 아이템이 없다면 어떤 재료가 와도 상관없으니 true
            {
                check[i] = true;
                continue;
            }
            else if (others[i] == null)                                                 //재료가 없다면 false
            {
                check[i] = false;
                break;
            }
            else if (items[i].Equals(others[i]) && items[i].amount <= others[i].amount) //아이템이 같고, 재료의 양이 레시피보다 같거나 많으면 true
            {
                check[i] = true;
                continue;
            }
            else                                                                        //그 외 false
            {
                check[i] = false;
                break;
            }
        }

        return ResultCheck();
    }

    bool ResultCheck()
    {
        for (int i = 0; i < check.Length; i++)
            if (!check[i]) return false;

        return true;
    }

    public bool ContainToRecipe(Item material)
    {
        if (material == null) return false;

        for(int i = 0; i < items.Length; i++) 
        {
            if (items[i] == null) continue;
            else if (items[i].Equals(material)) return true;
        }

        return false;
    }

    //public bool comparematerial(item[] others)
    //{
    //    if (referenceequals(others, null)) return false;
    //    if (referenceequals(product, null)) return false;

    //    for (int i = 0; i < items.length; i++)
    //    {
    //        if (items[i] == null)   //레시피 아이템이 없다면 어떤 재료가 와도 상관없으니 true;
    //        {
    //            check[i] = true;
    //            continue;
    //        }

    //        for (int j = 0; j < others.length; j++)
    //        {
    //            if (others[j] == null)  //재료가 없다면 false
    //            {
    //                check[i] = false;
    //                continue;
    //            }
    //            else if (items[i].equals(others[j]) && items[i].amount <= others[j].amount) //아이템이 같고, 재료의 양이 레시피보다 같거나 많으면 true
    //            {
    //                check[i] = true;
    //                break;
    //            }
    //            else //그 외 false
    //            {
    //                check[i] = false;
    //            }
    //        }
    //    }

    //    return resultcheck();
    //}
}

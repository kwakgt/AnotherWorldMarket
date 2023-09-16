using EnumManager;


public class Recipe
{
    WorkType workType;
    Item product;
    Item[] items = new Item[5];

    bool[] check = new bool[5];
    public Recipe(WorkType _workType, Item _product, Item item1, Item item2, Item item3, Item item4, Item item5)
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

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)   //레시피 아이템이 없다면 어떤 재료가 와도 상관없으니 true;
            {
                check[i] = true;
                continue;
            }

            for (int j = 0; j < others.Length; j++)
            {
                if (others[j] == null)  //재료가 없다면 false
                {
                    check[i] = false;
                    continue;
                }
                else if (items[i].Equals(others[j]) && items[i].amount <= others[j].amount) //아이템이 같고, 재료의 양이 레시피보다 같거나 많으면 true
                {
                    check[i] = true;
                    break;
                }
                else //그 외 false
                {
                    check[i] = false;
                }
            }
        }

        return ResultCheck();
    }


    bool ResultCheck()
    {
        int count = 0;
        for(int i= 0; i< check.Length; i++)
            if (check[i]) count++;

        return count == check.Length;
    }
}

using EnumManager;


public class Recipe
{
    public StaffWork workType { get;}
    public Item product { get;}
    public Item[] items { get; } = new Item[5];




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
        if (others == null) return false;
        if (product == null) return false;

        bool check = true;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)                                                       //������ �������� ���ٸ� � ��ᰡ �͵� ��������� true
            {
                check &= true;
                continue;
            }
            else if (others[i] == null)                                                 //��ᰡ ���ٸ� false
            {
                check &= false;
                break;
            }
            else if (items[i].Equals(others[i]) && items[i].amount <= others[i].amount) //�������� ����, ����� ���� �����Ǻ��� ���ų� ������ true
            {
                check &= true;
                continue;
            }
            else                                                                        //�� �� false
            {
                check &= false;
                break;
            }
        }

        return check;
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
    //        if (items[i] == null)   //������ �������� ���ٸ� � ��ᰡ �͵� ��������� true;
    //        {
    //            check[i] = true;
    //            continue;
    //        }

    //        for (int j = 0; j < others.length; j++)
    //        {
    //            if (others[j] == null)  //��ᰡ ���ٸ� false
    //            {
    //                check[i] = false;
    //                continue;
    //            }
    //            else if (items[i].equals(others[j]) && items[i].amount <= others[j].amount) //�������� ����, ����� ���� �����Ǻ��� ���ų� ������ true
    //            {
    //                check[i] = true;
    //                break;
    //            }
    //            else //�� �� false
    //            {
    //                check[i] = false;
    //            }
    //        }
    //    }

    //    return resultcheck();
    //}
}
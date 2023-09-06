using UnityEngine;


public class Shelf : Structure
{
    public int maxInvenSize { get; private set; }
    Item[] inventory;                     //매대 아이템 슬롯
    
    protected override void Awake()
    {
        base.Awake();
        maxInvenSize = frontSize;
        inventory = new Item[maxInvenSize];
    }
    
    protected override void Start()
    {
        base.Start();
        uniIndex = ShelfManager.instance.RequestShelfIndex();
        

        ShelfManager.instance.AddShelfDictionary(uniIndex, this); //현재 매대를 매니저에 추가

        //TEST
        //PutRandomItemInInven(0, 50);
        //PutRandomItemInInven(1, 50);
        //PutRandomItemInInven(2, 50);
        //PutRandomItemInInven(3, 50);
    }

    public Item FindItemInInven(Vector2 _shelfFrontPosition)   //선반 앞 위치로 매대 슬롯 가져오기
    {
        int index = FindIndexOfFrontPosition(_shelfFrontPosition);
        if (index > -1)
            return inventory[index];
        else
            return null;
    }

    public Item GetItemInInven(int index)
    {
        return inventory[index];
    }

    public void PutItemInInven(int index, Item newItem)
    {
        inventory[index] = newItem;
    }

    public void EmptyInventory(int index)
    {
        inventory[index] = null;
    }

    //TEST
    public void PutRandomItemInInven(int index, int amount)
    {
        if (inventory[index] == null)
        {
            inventory[index] = ItemManager.instance.GetRandomItem();
            inventory[index].PlusAmount(amount); 
        }
    }
}

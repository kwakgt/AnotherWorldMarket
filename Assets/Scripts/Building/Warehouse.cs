using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Warehouse : Structure
{
    public int maxInvenSize { get; private set; }                           //최대 인벤토리
    Item[] inventory;                       //인벤토리 슬롯
    Heap<Index> invenIdxs;                  //빈칸인 인벤 번호(인덱스)

    protected override void Awake()
    {
        base.Awake();
        maxInvenSize = 40;

        inventory = new Item[maxInvenSize];
        invenIdxs = new Heap<Index>(maxInvenSize);
        FillInventoryIndexFull();
    }

    protected override void Start()
    {
        base.Start();
        
        uniIndex = BuildingManager.instance.RequestWarehouseIndex();
        BuildingManager.instance.AddWarehouseDictionary(uniIndex, this);

        //TEST
        PutAllItemInInventory(2000);  //모든 아이템 창고에 넣기  
    }

    //TEST
    void PutAllItemInInventory(int amount)  //DB에 있는 모든 Item 창고에 넣기
    {
        int max = (ItemManager.instance.CountOfAllItem() < maxInvenSize) ? ItemManager.instance.CountOfAllItem() : maxInvenSize;    //아이템 개수와 전체 창고칸수 비교하여 작은걸 max값으로
        for (int i = 0; i < max; i++)
        {
            int index = invenIdxs.RemoveFirst().Value;
            inventory[index] = ItemManager.instance.GetItem(i);
            inventory[index].PlusAmount(amount);
        }
    }

    public int FirstEmptyIndexInInventory()
    {
        if (IsEmptyInInventory())   return invenIdxs.RemoveFirst().Value;
        else                        return -1;
    }

    public bool IsEmptyInInventory()
    {
        if (invenIdxs.Count > 0)    return true;
        else                        return false;
    }

    public int FindItemIndexInInventory(Item itemToFind, bool putItemInWarehouse = false)
    {
        if (itemToFind == null) return -1;

        if (putItemInWarehouse)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null && inventory[i].Equals(itemToFind) && inventory[i].amount < inventory[i].amountOfWarehouse)
                    return i;
            }
        }
        else
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null && inventory[i].Equals(itemToFind))
                    return i;
            }
        }
        return -1;
    }

    public Item GetItemInInven(int index = -1)
    {
        if (invenIdxs.Count == maxInvenSize) return null;

        if(index == -1)
        {
            Item random;
            while (true)
            {
                random = inventory[Random.Range(0, inventory.Length)];
                if (random != null && random.amountOfShelf > 0)
                {
                    return random;
                }
            }
        }

        return inventory[index];
    }   

    public void PutItemInInven(int index, Item newItem)
    {
        inventory[index] = newItem;
    }

    public void EmptyInventory(int index)
    {
        Index value = new Index(index);
        invenIdxs.Add(value);
        inventory[index] = null;
    }

    void FillInventoryIndexFull()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            Index index = new Index(i);
            invenIdxs.Add(index);
        }
    } 
}

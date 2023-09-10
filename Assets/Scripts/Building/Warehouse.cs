using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Warehouse : Structure
{
    public int maxInvenSize { get; private set; }                           //최대 인벤토리
    Item[] inventory;                    //인벤토리 슬롯
    Heap<Index> invenIdxs;                      //빈칸인 인벤 번호(인덱스)

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
        
        uniIndex = WarehouseManager.instance.RequestWarehouseIndex();

        //TEST
        WarehouseManager.instance.AddWarehouseList(this);
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

    public Vector2 GetRandomWarehouseFrontPosition(Vector2 currPosition)
    {
        Vector2 randomPosition = frontPositions[Random.Range(0, frontSize)];
        bool contains = false;
        for (int i = 0; i < frontSize; i++) //currPosition이 창고 입구에 포함되는지 확인
        {
            if (frontPositions[i] == currPosition)
            {
                contains = true;
                break;
            }
        }

        while (contains && currPosition == randomPosition)              //currPosition이 입구에 포함되고 현재 위치와 랜덤입구위치가 같으면
        {
            randomPosition = frontPositions[Random.Range(0, frontSize)]; //다른 입구로 변경하기
        }
        return randomPosition;
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

using UnityEngine;


public class Warehouse : Structure
{
    public int maxInvenSize { get; private set; }                           //�ִ� �κ��丮
    Item[] inventory;                    //�κ��丮 ����
    Heap<Index> invenIdxs;                      //��ĭ�� �κ� ��ȣ(�ε���)

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
        PutAllItemInInventory(1000);  //��� ������ â���� �ֱ�  
    }

    //TEST
    void PutAllItemInInventory(int amount)  //DB�� �ִ� ��� Item â���� �ֱ�
    {
        int max = (ItemManager.instance.CountOfAllItem() < maxInvenSize) ? ItemManager.instance.CountOfAllItem() : maxInvenSize;    //������ ������ ��ü â��ĭ�� ���Ͽ� ������ max������
        for (int i = 0; i < max; i++)
        {
            int index = invenIdxs.RemoveFirst().Value;
            inventory[index] = ItemManager.instance.GetItem(i);
            inventory[index].PlusAmount(amount);
        }
    }

    public Vector2 GetWarehouseFrontPosition(Vector2 currPosition)
    {
        Vector2 randomPosition = frontPositions[Random.Range(0, frontSize)];
        bool contains = false;
        for (int i = 0; i < frontSize; i++) //currPosition�� â�� �Ա��� ���ԵǴ��� Ȯ��
        {
            if (frontPositions[i] == currPosition)
            {
                contains = true;
                break;
            }
        }

        while (contains && currPosition == randomPosition)              //currPosition�� �Ա��� ���Եǰ� ���� ��ġ�� �����Ա���ġ�� ������
        {
            randomPosition = frontPositions[Random.Range(0, frontSize)]; //�ٸ� �Ա��� �����ϱ�
        }
        return randomPosition;
    }

    public int FirstEmptyIndexInInventory()
    {
        if (invenIdxs.Count == 0)
            return -1;
        else
            return invenIdxs.RemoveFirst().Value;
    }

    public int FindItemIndexInInventory(Item itemToFind)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(itemToFind))
                return i;
        }

        return -1;
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
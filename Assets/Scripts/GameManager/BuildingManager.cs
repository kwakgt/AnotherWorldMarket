using EnumManager;
using System.Collections.Generic;
using UnityEngine;


public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    //Dictionary
    //Remove�� ���� �����ǰ� Ű�� �����Ѵ�.
    
    //Shelf Dictionary
    Dictionary<int, Shelf> shelfDictionary = new Dictionary<int, Shelf>();  //�Ŵ� �����ε���, Shelf
    Queue<int> shelfIndexQue = new Queue<int>();                           //������ �Ŵ��� �����ε��� ����
    int shelfIndex;                                                        //�����ε���
    
    //Warehouse Dictionary
    Dictionary<int, Warehouse> warehouseDictionary = new Dictionary<int, Warehouse>();
    Queue<int> warehouseIndexQue = new Queue<int>();
    int warehouseIndex;

    //Factory Dictionary
    Dictionary<int, Factory> factoryDictionary = new Dictionary<int, Factory>();
    Dictionary<StaffWork, List<Factory>> factoryWorkTypeDictionary = new Dictionary<StaffWork, List<Factory>>();
    Queue<int> factoryIndexQue = new Queue<int>();
    int factoryIndex;

    void Awake()
    {
        instance = this;
        InitFactoryWorkTypeDictionnary();
    }
   
    //Shelf ================================================================================================================================================================================
    public int RequestShelfIndex() //�����ε��� �ο�
    {
        if(shelfIndexQue.Count > 0)    //�ε��� ť�� ���� �ε����� ������ ��Ȱ��
        {
            return shelfIndexQue.Dequeue();
        }

        return shelfIndex++;
    }

    public Shelf RequestRandomShelf()          //���� �Ŵ� Ÿ�� �ο�
    {
        if (shelfDictionary.Count == 0) return null; //�Ŵ��Ͽ� ���� ������ ����

        int index = Random.Range(0, shelfIndex);
        while(!shelfDictionary.ContainsKey(index))  //index�� ���ٸ� ���û
        {
            index = Random.Range(0, shelfIndex);
        }
        Shelf shelf;
        shelfDictionary.TryGetValue(index, out shelf);

        return shelf;
    }

    public void AddShelfDictionary(int index, Shelf shelf)
    {
        if(shelfDictionary.ContainsKey(index))
            shelfDictionary[index] = shelf;
        else
            shelfDictionary.Add(index, shelf);
    }

    public void RemoveShelfDictionary(int index)
    {
        shelfDictionary.Remove(index);
        shelfIndexQue.Enqueue(index);
    }

    //Warehouse ================================================================================================================================================================================
    public int RequestWarehouseIndex()
    {
        if (warehouseIndexQue.Count > 0)    //�ε��� ť�� ���� �ε����� ������ ��Ȱ��
        {
            return warehouseIndexQue.Dequeue();
        }

        return warehouseIndex++;
    }

    public Warehouse RequestRandomWarehouse()
    {
        if (warehouseDictionary.Count == 0) return null;

        int index = Random.Range(0, warehouseIndex);
        while (!warehouseDictionary.ContainsKey(index))  //index�� ���ٸ� ���û
        {
            index = Random.Range(0, warehouseIndex);
        }
        Warehouse warehouse;
        warehouseDictionary.TryGetValue(index, out warehouse);

        return warehouse;
    }

    public Warehouse FindItemInWarehouseList(Item itemToFind)
    {
        foreach(Warehouse warehouse in warehouseDictionary.Values)
        {
            if (warehouse.FindItemIndexInInventory(itemToFind) > 0)
                return warehouse;
        }

        return null;
    }

    public Warehouse FindEmptyWarehouse()
    {
        foreach (Warehouse warehouse in warehouseDictionary.Values)
        {
            if (warehouse.IsEmptyInInventory())
                return warehouse;
        }

        return null;
    }

    public Item GetItemInRandomWarehouse()
    {
        //�ǸŴ뿡 �������� ������ üũ �۾� �� null ��� ������ �������� �ֱ����� �Լ�
        return RequestRandomWarehouse().GetItemInInven();
        
    }

    public void AddWarehouseDictionary(int index, Warehouse warehouse)
    {
        if(warehouseDictionary.ContainsKey(index))
            warehouseDictionary[index] = warehouse;
        else
            warehouseDictionary.Add(index, warehouse);
    }

    public void RemoveWarehouseDictionary(int index)
    {
        warehouseDictionary.Remove(index);
        warehouseIndexQue.Enqueue(index);
    }

    //Factory   ================================================================================================================================================================================
    void InitFactoryWorkTypeDictionnary()
    {
        factoryWorkTypeDictionary.Add(StaffWork.Cooking, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Cutting, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Drying, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Juicing, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Melting, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Mixing, new List<Factory>());
        factoryWorkTypeDictionary.Add(StaffWork.Packaging, new List<Factory>());
    }

    public int RequestFactoryIndex()
    {
        if (factoryIndexQue.Count > 0)    //�ε��� ť�� ���� �ε����� ������ ��Ȱ��
        {
            return factoryIndexQue.Dequeue();
        }

        return factoryIndex++;
    }

    public Factory RequestEmptyFactory(StaffWork work)
    {
        List<Factory> list = factoryWorkTypeDictionary[work];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].IsEmptyStaff())
                return list[i];
        }
        return null;
    }

    public Factory RequestRandomFactory()
    {
        return factoryDictionary[Random.Range(0, factoryDictionary.Count)];
    }

    public void AddFactoryDictionary(int index, StaffWork workType, Factory factory)
    {
        factoryWorkTypeDictionary[workType].Add(factory);
        if (factoryDictionary.ContainsKey(index))
            factoryDictionary[index] = factory;
        else
            factoryDictionary.Add(index, factory);
    }

    public void RemoveWarehouseDictionary(int index, StaffWork workType, Factory factory)
    {
        factoryWorkTypeDictionary[workType].Remove(factory);
        factoryDictionary.Remove(index);
        factoryIndexQue.Enqueue(index);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    public static WarehouseManager instance;

    //List
    //Remove�� �ڿ� �ִ� ��ҵ��� �˾Ƽ� ��ĭ�� �Ű�����. 
    List<Warehouse> warehouseList = new List<Warehouse>();
    int uniqueIndex;

    void Awake()
    {
        instance = this;
    }

    public int RequestWarehouseIndex()
    {
        return uniqueIndex++;
    }

    public Warehouse RequestRandomWarehouse()
    {
        return warehouseList[Random.Range(0, warehouseList.Count)];
    }

    public Warehouse FindItemInWarehouseList(Item itemToFind)
    {
        for (int i = 0; i < warehouseList.Count; i++)
        {
            if(warehouseList[i].FindItemInWarehouse(itemToFind))
                return warehouseList[i];
        }
        return null;
    }

    public void AddWarehouseList(Warehouse warehouse)
    {
        warehouseList.Add(warehouse);
    }

    public void RemoveWarehouseList(Warehouse warehosue)
    {
        warehouseList.Remove(warehosue);
    }

    public void UpdateWarehouseList(Warehouse warehouse)
    {
        Warehouse house = warehouseList.Find(x => x == warehouse);  //����Ʈ���� ���ǿ� �´� �� ã��
        house = warehouse;
    }

    
}
using System.Collections.Generic;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    public static WarehouseManager instance;

    //List
    //Remove시 뒤에 있는 요소들이 알아서 한칸씩 옮겨진다. 
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
            if(warehouseList[i].FindItemIndexInInventory(itemToFind) > -1)
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
        //TODO:: 창고 인덱스 부여, 창고 인덱스로 Equals 함수 만들기
        for (int i = 0; i < warehouseList.Count; i++)
        {
            if (warehouseList[i] == warehouse)
                warehouseList[i] = warehouse;
        }
    }

    
}

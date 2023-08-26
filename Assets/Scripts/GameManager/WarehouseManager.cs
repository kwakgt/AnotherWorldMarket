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
        Warehouse house = warehouseList.Find(x => x == warehouse);  //리스트에서 조건에 맞는 값 찾기
        house = warehouse;
    }

    
}

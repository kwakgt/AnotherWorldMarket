using EnumManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehousePanel : MonoBehaviour
{
    Transform invenPanel;

    ItemSlot[] itemSlot;

    void Awake()
    {
        invenPanel = transform.GetChild(0);
        itemSlot = invenPanel.GetComponentsInChildren<ItemSlot>();
    }
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        Display();
    }

    void Display()
    {
        Warehouse warehouse = GameManager.instance.selectedWarehouse;
        if (warehouse == null) return;   //선택된 유닛이 없으면 종료

        for (int i = 0; i < warehouse.maxInvenSize; i++)
        {
            itemSlot[i].SetItem(warehouse.GetItemInInven(i));
        }
    }

    //public void SetUIManager()
    //{
    //    UIManager.instance.panelOnOff += OnOff;
    //}

    //public void OnOff(PanelName panel)
    //{
    //    if (PanelName == panel)
    //        gameObject.gameObject.SetActive(true);
    //    else
    //        gameObject.gameObject.SetActive(false);
    //}
}

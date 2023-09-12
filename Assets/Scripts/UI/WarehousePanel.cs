using EnumManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehousePanel : MonoBehaviour
{
    Transform invenPanel;

    Image[] invenImage;

    void Awake()
    {
        invenPanel = transform.GetChild(0);
        invenImage = invenPanel.GetComponentsInChildren<Image>();   //InvenPanel의 Image까지 포함됨
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

        for (int i = 1; i < warehouse.maxInvenSize + 1; i++)    //1부터 시작하므로 범위에 +1해준다, 0은 부모 Image여서 제외
        {
            if (warehouse.GetItemInInven(i - 1) != null)                  //unit 인벤트로니는 0부터
            {
                invenImage[i].sprite = warehouse.GetItemInInven(i - 1).sprite;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = warehouse.GetItemInInven(i - 1).amount.ToString();
            }
            else
            {
                invenImage[i].sprite = null;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = null;
            }
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

using EnumManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour, IPanelOnOff
{
    Transform invenPanel;
    ItemSlot[] itemSlot;

    Transform infoPanel;
    TextMeshProUGUI infoText;

    public PanelName PanelName { get { return PanelName.UnitPanel; } }

    void Awake()
    {
        invenPanel = transform.GetChild(0);
        itemSlot = invenPanel.GetComponentsInChildren<ItemSlot>();   //InvenPanel의 Image까지 포함되어 Length = 13;

        infoPanel = transform.GetChild(1);
        infoText = infoPanel.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false); //MouseController에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    void Update()
    {
        Display();
    }

    void Display()
    {
        Unit unit = GameManager.instance.selectedUnit;
        if (unit == null) return;   //선택된 유닛이 없으면 종료

        //정보 패널
        if (unit.CompareTag("Customer"))
        {
            Customer customer = (Customer)unit;
            infoText.text = "shoppingCount: " + customer.shoppingCount + "\nmoney : " + customer.money;     //test
        }
        else
            infoText.text = null;

        //인벤토리 패널
        for(int i = 0; i < unit.invenSizeAvailable; i++)
        {
            itemSlot[i].SetItem(unit.GetItemInInven(i));
        }

        for(int i = unit.invenSizeAvailable; i < itemSlot.Length; i++ )
        {
            itemSlot[i].SetUnusableItemSlot();
        }
    }

    public void SetUIManager()
    {
        UIManager.instance.selectedPanelOnOff += OnOff;
    }

    public void OnOff(PanelName panel)
    {
        if(panel == PanelName)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}

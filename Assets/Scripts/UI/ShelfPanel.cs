using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumManager;
using System.Runtime.InteropServices.WindowsRuntime;

public class ShelfPanel : MonoBehaviour, IPanelOnOff
{
    Transform invenPanel;

    ItemSlot[] itemSlot;

    public PanelName PanelName { get { return PanelName.ShelfPanel; } }

    void Awake()
    {
        invenPanel = transform.GetChild(0);
        itemSlot = invenPanel.GetComponentsInChildren<ItemSlot>();   //InvenPanel의 Image까지 포함됨
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false);   //MouseController에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    void Update()
    {
        Display();
    }

    void Display()
    {
        Shelf shelf = GameManager.instance.selectedShelf;
        if (shelf == null) return;   //선택된 유닛이 없으면 종료

        for (int i = 0; i < shelf.maxInvenSize; i++)    //1부터 시작하므로 범위에 +1해준다, 0은 부모 Image여서 제외
        {
            itemSlot[i].SetItem(shelf.GetItemInInven(i));
        }
    }

    public void SetUIManager()
    {
        UIManager.instance.selectedPanelOnOff += OnOff;
    }

    public void OnOff(PanelName panel)
    {
        if (PanelName == panel)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}

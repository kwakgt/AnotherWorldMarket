using EnumManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaffManagementPanel : MonoBehaviour, IPanelOnOff
{
    public PanelName PanelName { get { return PanelName.StaffManagementPanel; } }

    //스탭관리슬롯 프리팹
    public GameObject staffManagementSlotPrefab;
    public GameObject contents; //프리팹 부모오브젝트

    Transform topPanel;
    Transform staffListPanel;

    List<StaffManagementSlot> staffSlots;

    void Awake()
    {
        topPanel = transform.GetChild(0);
        //TODO:: TOP패널 추가


        staffListPanel = transform.GetChild(1);
        staffSlots = staffListPanel.GetComponentsInChildren<StaffManagementSlot>().ToList();
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false); //MenuBottonPanel에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    void Update()
    {
        SetStaffList();
    }

    void SetStaffList()
    {
        for (int i = 0; i < UnitManager.instance.staffList.Count; i++)
        {
            if(i < staffSlots.Count) //슬롯이 남아있다면
            {
                staffSlots[i].SetStaff(UnitManager.instance.staffList[i]);
                staffSlots[i].gameObject.SetActive(true);
            }
            else
            {
                StaffManagementSlot slot = Instantiate(staffManagementSlotPrefab, contents.transform).GetComponent<StaffManagementSlot>();
                slot.SetStaff(UnitManager.instance.staffList[i]);
                staffSlots.Add(slot);
            }
        }

        for (int i = UnitManager.instance.staffList.Count; i < staffSlots.Count; i++)
        {
            staffSlots[i].gameObject.SetActive(false);
        }
    }

    public void SetUIManager()
    {
        UIManager.instance.panelOnOff += OnOff;
    }

    public void OnOff(PanelName panel)
    {
        if (PanelName == panel)
            gameObject.gameObject.SetActive(true);
        else
            gameObject.gameObject.SetActive(false);
    }
}

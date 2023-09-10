using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaffManagementPanel : MonoBehaviour
{
    //���ǰ������� ������
    public GameObject staffManagementSlotPrefab;
    public GameObject contents; //������ �θ������Ʈ

    Transform topPanel;
    Transform staffListPanel;

    List<StaffManagementSlot> staffSlots;
    void Awake()
    {
        topPanel = transform.GetChild(0);
        //TODO:: TOP�г� �߰�


        staffListPanel = transform.GetChild(1);
        staffSlots = staffListPanel.GetComponentsInChildren<StaffManagementSlot>().ToList();
    }

    void Start()
    {
        gameObject.SetActive(false); //MenuBottonPanel���� GameObject.Find�Լ� ����� ���� Start���� ��Ȱ��ȭ, ��Ȱ��ȭ�� ������Ʈ�� Find�Լ��� ���� �ȵ�.
    }

    void Update()
    {
        SetStaffList();
    }

    void SetStaffList()
    {
        for (int i = 0; i < UnitManager.instance.staffList.Count; i++)
        {
            if(i < staffSlots.Count) //������ �����ִٸ�
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

}
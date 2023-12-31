using EnumManager;
using System;
using TMPro;
using UnityEngine;

public class StaffManagementSlot : MonoBehaviour
{
    //유닛슬롯 1개, 차원드롭다운 1개, 스탯슬롯 리스트로 구성

    Staff staff;
    UnitSlot unitSlot;
    StatSlot[] statSlots;
    TMP_Dropdown dimensionDropdown;

    void Awake()
    {
        unitSlot = GetComponentInChildren<UnitSlot>();
        statSlots = GetComponentsInChildren<StatSlot>();
        dimensionDropdown = GetComponentInChildren<TMP_Dropdown>();
    }

    public void SetStaff(Staff _staff)
    {
        staff = _staff;

        unitSlot.SetStaff(staff);

        int i = 0;

        foreach (StaffWork work in Enum.GetValues(typeof(StaffWork)))
        {
            if (staff.IsDimensionWork(work) || staff.IsFactoryWork(work) || work == StaffWork.Carrying || work == StaffWork.Deliverying)
            {
                statSlots[i].SetStaff(staff, work);
                i++;
            }
        }

        //차원패널에서 차원이동 시 드롭다운 차원 동기화
        dimensionDropdown.captionText.text = Enum.GetName(typeof(Dimension), staff.dimension);
        dimensionDropdown.captionImage.sprite = SpriteManager.instance.GetDimensionImage(staff.dimension);
        dimensionDropdown.value = (int)staff.dimension;
    }

    public void OnChangeValue() //드롭다운 이벤트
    {
        //드롭다운에서 선택된 차원으로 이동
        staff.ShiftDimension(Enum.Parse<Dimension>(dimensionDropdown.options[dimensionDropdown.value].text));
    }
}

using UnityEngine;
using EnumManager;
using System;
using TMPro;
using UnityEngine.UIElements;

public class StaffManagementSlot : MonoBehaviour
{
    Staff staff;
    UnitSlot unitSlot;
    StatSlot[] statSlots;
    TMP_Dropdown dropdown;

    void Awake()
    {
        unitSlot = GetComponentInChildren<UnitSlot>();
        statSlots = GetComponentsInChildren<StatSlot>();
        dropdown = GetComponentInChildren<TMP_Dropdown>();
    }

    public void SetStaff(Staff _staff)
    {
        staff = _staff;

        unitSlot.SetStaff(staff);

        statSlots[0].SetStaff(staff, WorkType.Purchase);
        statSlots[1].SetStaff(staff, WorkType.Carrying);
        statSlots[2].SetStaff(staff, WorkType.Felling);
        statSlots[3].SetStaff(staff, WorkType.Mining);
        statSlots[4].SetStaff(staff, WorkType.Collecting);
        statSlots[5].SetStaff(staff, WorkType.Hunting);
        statSlots[6].SetStaff(staff, WorkType.Fishing);
        //TODO:: 스탯 추가

        //차원패널에서 차원이동 시 드롭다운 차원 동기화
        dropdown.captionText.text = Enum.GetName(typeof(Dimension), staff.dimension);
        dropdown.captionImage.sprite = SpriteIconManager.instance.GetDimensionIcon(staff.dimension);
        dropdown.value = (int)staff.dimension;
    }

    public void OnChangeValue() //드롭다운 이벤트
    {
        //드롭다운에서 선택된 차원으로 이동
        staff.ShiftDimension(Enum.Parse<Dimension>(dropdown.options[dropdown.value].text));
    }
}

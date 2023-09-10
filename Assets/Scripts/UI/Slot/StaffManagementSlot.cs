using UnityEngine;
using EnumManager;
using UnityEngine.UI;
using System;

public class StaffManagementSlot : MonoBehaviour
{
    UnitSlot unitSlot;
    StatSlot[] statSlots;
    Dropdown dropdown;

    void Awake()
    {
        unitSlot = GetComponentInChildren<UnitSlot>();
        statSlots = GetComponentsInChildren<StatSlot>();
        dropdown = GetComponentInChildren<Dropdown>();
    }

    public void SetStaff(Staff staff)
    {
        unitSlot.SetStaff(staff);

        statSlots[0].SetStaff(staff, WorkType.Purchase);
        statSlots[1].SetStaff(staff, WorkType.Carrying);
        statSlots[2].SetStaff(staff, WorkType.Felling);
        statSlots[3].SetStaff(staff, WorkType.Mining);
        statSlots[4].SetStaff(staff, WorkType.Collecting);
        statSlots[5].SetStaff(staff, WorkType.Hunting);
        statSlots[6].SetStaff(staff, WorkType.Fishing);
        //TODO:: Ω∫≈» √ﬂ∞°

    }

    void Dropdown()
    {
        Dimension a = Enum.Parse<Dimension>(dropdown.options[dropdown.value].text);
    }
}

using UnityEngine;
using EnumManager;

public class StaffManagementSlot : MonoBehaviour
{
    UnitSlot unitSlot;
    StatSlot[] statSlots;

    void Awake()
    {
        unitSlot = GetComponentInChildren<UnitSlot>();
        statSlots = GetComponentsInChildren<StatSlot>();
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
}

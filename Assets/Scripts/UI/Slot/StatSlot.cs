using EnumManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlot : MonoBehaviour
{
    Staff staff;
    WorkType workType;


    Image workImage;
    TextMeshProUGUI stat;

    void Awake()
    {
        workImage = transform.GetChild(0).GetComponent<Image>();
        stat = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Display();
    }

    public void SetStaff(Staff _staff, WorkType _workType)
    {
        staff = _staff;
        workType = _workType;
    }

    void Display()
    {
        if (staff != null)
        {
            //TODO:: 작업이미지 추가
            stat.text = staff.stat.ReplaceFromWorkTypeToInt(workType).ToString();
        }

        //staff == null 이면 StaffManagementSlot이 파괴된다.
    }
}

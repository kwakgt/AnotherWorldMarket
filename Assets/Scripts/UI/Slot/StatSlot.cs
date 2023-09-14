using EnumManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlot : MonoBehaviour
{
    //작업이미지, 작업버튼, 스탯텍스트 각각 1개로 구성

    Staff staff;
    WorkType workType;


    Image workImage;
    Button workButton;
    TextMeshProUGUI stat;

    void Awake()
    {
        workImage = transform.GetChild(0).GetComponent<Image>();
        workButton = transform.GetChild(0).GetComponent<Button>();
        stat = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Display();

        //추가할 함수에 인수가 존재하면 델리게이트 또는 람다식을 사용한다.
        //버튼 클릭시 작동
        workButton.onClick.AddListener(() => staff.ReceiveCommand(workType));
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
            workImage.sprite = SpriteIconManager.instance.GetWorkIcon(workType);
            stat.text = staff.stat.ReplaceFromWorkTypeToInt(workType).ToString();
        }

        //staff == null 이면 StaffManagementSlot이 파괴되거나 disable
    }
}

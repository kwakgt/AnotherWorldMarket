using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonPanel : MonoBehaviour
{
    //스탭관리패널
    GameObject staffManagermentPanel;
    
    //TODO:: 메뉴 추가


    void Awake()
    {
        staffManagermentPanel = GameObject.Find("StaffManagementPanel");
    }

    void Update()
    {
        if(InputManager.instance.fKeyDown)
        {
            InputManager.instance.fKeyDown = false;
            OnOffStaffManagermentPanel();
        }
    }

    //스탭관리패널 On/OFF
    public void OnOffStaffManagermentPanel() //버튼용
    {
        if (staffManagermentPanel.activeSelf)
            staffManagermentPanel.SetActive(false);
        else
            staffManagermentPanel.SetActive(true);
    }
}

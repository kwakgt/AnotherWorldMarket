using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonPanel : MonoBehaviour
{
    //스탭관리패널
    GameObject staffManagermentPanel;

    //차원패널
    GameObject dimensionPanel;
    
    //TODO:: 메뉴 추가


    void Awake()
    {
        staffManagermentPanel = GameObject.Find("StaffManagementPanel");
        dimensionPanel = GameObject.Find("DimensionPanel");
    }

    void Update()
    {
        if(InputManager.instance.fKeyDown)
        {
            InputManager.instance.fKeyDown = false;
            OnOffStaffManagermentPanel();
        }
        else if(InputManager.instance.eKeyDown)
        {
            InputManager.instance.eKeyDown = false;
            OnOffDimensionPanel();
        }
    }

    //스탭관리패널 On/Off
    public void OnOffStaffManagermentPanel() //버튼용
    {
        if (staffManagermentPanel.activeSelf)
            staffManagermentPanel.SetActive(false);
        else
            staffManagermentPanel.SetActive(true);
    }

    //차원패널 On/Off
    public void OnOffDimensionPanel() //버튼용
    {
        if(dimensionPanel.activeSelf)
            dimensionPanel.SetActive(false);
        else
            dimensionPanel.SetActive(true);
    }
}

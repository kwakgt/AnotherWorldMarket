using UnityEngine;
using EnumManager;

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
            UIManager.instance.ExecutePanelOnOFF(PanelName.Off);
        else
            UIManager.instance.ExecutePanelOnOFF(PanelName.StaffManagementPanel);
    }

    //차원패널 On/Off
    public void OnOffDimensionPanel() //버튼용
    {
        if(dimensionPanel.activeSelf)
            UIManager.instance.ExecutePanelOnOFF(PanelName.Off);
        else
            UIManager.instance.ExecutePanelOnOFF(PanelName.DimensionPanel);
    }
}

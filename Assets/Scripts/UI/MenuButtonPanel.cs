using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonPanel : MonoBehaviour
{
    //���ǰ����г�
    GameObject staffManagermentPanel;
    
    //TODO:: �޴� �߰�


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

    //���ǰ����г� On/Off
    public void OnOffStaffManagermentPanel() //��ư��
    {
        if (staffManagermentPanel.activeSelf)
            staffManagermentPanel.SetActive(false);
        else
            staffManagermentPanel.SetActive(true);
    }
}
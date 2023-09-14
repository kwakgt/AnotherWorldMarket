using EnumManager;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public event Action<PanelName> panelOnOff;
    public bool IsAllOff { get; private set; } = true;

    void Awake()
    {
        instance = this;
    }

    public void ExecutePanelOnOFF(PanelName panel)
    {
        panelOnOff(panel);
        if(panel == PanelName.Off)
            IsAllOff = true;
        else
            IsAllOff = false;
    }
}

public interface IPanelOnOff
{
    PanelName PanelName { get; }

    /// <summary>
    /// UI�Ŵ��� panelOnOff ��������Ʈ�� ���
    /// </summary>
    void SetUIManager();
    
    /// <summary>
    /// �гθ� ���Ͽ� �ڱ��ڽ��̸� true, �ƴϸ� False
    /// </summary>
    void OnOff(PanelName panel);
}
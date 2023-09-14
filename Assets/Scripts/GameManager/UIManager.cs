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

    public void ExecutePanelOnOff(PanelName panel)
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
    /// UI매니저 panelOnOff 델리게이트에 등록
    /// </summary>
    void SetUIManager();
    
    /// <summary>
    /// 패널명 비교하여 자기자신이면 true, 아니면 False
    /// </summary>
    void OnOff(PanelName panel);
}
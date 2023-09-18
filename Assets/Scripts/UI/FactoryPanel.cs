using EnumManager;
using System;
using TMPro;
using UnityEngine;

public class FactoryPanel : MonoBehaviour, IPanelOnOff
{
    Transform topPanel;
    Transform recipePanel;
    Transform inventory;

    TextMeshProUGUI nameText;

    public PanelName PanelName { get { return PanelName.FactoryPanel; } }

    void Awake()
    {
        topPanel = transform.GetChild(0);
        nameText = topPanel.GetComponentInChildren<TextMeshProUGUI>();
        
        recipePanel = transform.GetChild(1);
        inventory = transform.GetChild(2);
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false);
    }

    void Display()
    {
        Factory factory = GameManager.instance.selectedFactory;
        nameText.text = Enum.GetName(typeof(StaffWork), factory.workType) + " Factory";
    }

    public void SetUIManager()
    {
        UIManager.instance.selectedPanelOnOff += OnOff;
    }

    public void OnOff(PanelName panel)
    {
        if(panel == PanelName)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}

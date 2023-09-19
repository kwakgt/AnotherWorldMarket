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
    RecipeSlot[] recipeSlots;
    ItemSlot[] itemSlots;

    public PanelName PanelName { get { return PanelName.FactoryPanel; } }

    void Awake()
    {
        topPanel = transform.GetChild(0);
        nameText = topPanel.GetComponentInChildren<TextMeshProUGUI>();
        
        recipePanel = transform.GetChild(1);
        recipeSlots = recipePanel.GetComponentsInChildren<RecipeSlot>();

        inventory = transform.GetChild(2);
        itemSlots = inventory.GetComponentsInChildren<ItemSlot>();
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        SetFactory();
    }

    void Update()
    {
        Display();
    }

    void SetFactory()
    {
        Factory factory = GameManager.instance.selectedFactory;
        if (factory == null) return;
        nameText.text = Enum.GetName(typeof(StaffWork), factory.workType) + " Factory";

        for (int i = 0;  i < recipeSlots.Length; i++)
        {
            recipeSlots[i].SetFactory(i, factory);
        }
    }

    void Display()
    {
        Factory factory = GameManager.instance.selectedFactory;
        if(factory == null) return;
        for (int i = 0; i < factory.InventorySize; i++)
        {
            itemSlots[i].SetItem(factory.GetItemInInventory(i));
        }

        for (int i = factory.InventorySize; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetItem(null);
        }
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

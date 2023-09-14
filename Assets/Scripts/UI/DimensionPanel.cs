using EnumManager;
using System;
using UnityEngine;

public class DimensionPanel : MonoBehaviour, IPanelOnOff
{
    public PanelName PanelName { get { return PanelName.DimensionPanel; } }
    
    DimensionSlot[] dimensionSlot;

    //TODO:: ���� ���ſ� ���� �̿뿩�� �߰�

    void Awake()
    {
        dimensionSlot = GetComponentsInChildren<DimensionSlot>();
        SetDimensionSlot();
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false); //MenuBottonPanel���� GameObject.Find�Լ� ����� ���� Start���� ��Ȱ��ȭ, ��Ȱ��ȭ�� ������Ʈ�� Find�Լ��� ���� �ȵ�.
    }

    /// <summary>
    /// DimensionSlot�� dimension���� �־��ָ� �������� DimensionSlot���� ������ �����ش�.
    /// </summary>
    void SetDimensionSlot()
    {
        int i = 0;
        foreach (Dimension dimension in Enum.GetValues(typeof(Dimension)))
        {
            dimensionSlot[i++].Dimension = dimension;
        }
    }

    /// <summary>
    /// �����гο��� �����̵��� �� �� �ʿ�, ���ֽ��� �巡�װ� ������ ���� ������ ��ȯ
    /// </summary>
    public bool DimensionContainsScreenPoint(Vector2 screenPoint, out Dimension dimension)
    {
        foreach (var slot in dimensionSlot)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), screenPoint))
            {
                dimension = slot.Dimension;
                return true;
            }
        }

        dimension = Dimension.Astaria;
        return false;
    }

    public void SetUIManager()
    {
        UIManager.instance.panelOnOff += OnOff;
    }

    public void OnOff(PanelName panel)
    {
        if (PanelName == panel)
            gameObject.gameObject.SetActive(true);
        else
            gameObject.gameObject.SetActive(false);
    }
}
using EnumManager;
using System;
using UnityEngine;

public class DimensionPanel : MonoBehaviour, IPanelOnOff
{
    public PanelName PanelName { get { return PanelName.DimensionPanel; } }
    
    DimensionSlot[] dimensionSlot;

    //TODO:: 차원 구매에 따른 이용여부 추가

    void Awake()
    {
        dimensionSlot = GetComponentsInChildren<DimensionSlot>();
        SetDimensionSlot();
    }

    void Start()
    {
        SetUIManager();
        gameObject.SetActive(false); //MenuBottonPanel에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    /// <summary>
    /// DimensionSlot의 dimension값만 넣어주면 나머지는 DimensionSlot에서 정보를 보여준다.
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
    /// 차원패널에서 차원이동을 할 때 필요, 유닛슬롯 드래그가 끝났을 때의 차원을 반환
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
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}

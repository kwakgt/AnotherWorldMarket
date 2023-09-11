using EnumManager;
using UnityEngine;
using System;

public class DimensionPanel : MonoBehaviour
{
    DimensionSlot[] dimensionSlot;

    //TODO:: 차원 구매에 따른 이용여부 추가
    
    void Awake()
    {
        dimensionSlot = GetComponentsInChildren<DimensionSlot>();
        SetDimensionSlot();
    }

    void Start()
    {
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
            dimensionSlot[i++].SetDimension(dimension);
        }
    }
}

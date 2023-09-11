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

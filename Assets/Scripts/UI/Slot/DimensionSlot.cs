using EnumManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DimensionSlot : MonoBehaviour
{
    public GameObject unitSlotPrefab;   //유닛슬롯프리팹
    public GameObject contents;          //프리팹 부모오브젝트

    Dimension dimension;

    Button button;
    List<UnitSlot> unitSlots;
    void Awake()
    {
        button = GetComponentInChildren<Button>();
        unitSlots = GetComponentsInChildren<UnitSlot>().ToList();
    }

    void Start()
    {
        SetButton();
    }

    void Update()
    {
        SetUnitList();
    }

    public void SetDimension(Dimension _dimension)
    {
        dimension = _dimension;
    }

    void SetUnitList()
    {
        for (int i = 0; i < DimensionManager.instance.dimensions[dimension].Count; i++)
        {
            if (i < unitSlots.Count) //슬롯이 남아있다면
            {
                unitSlots[i].SetStaff(DimensionManager.instance.dimensions[dimension][i]);
                unitSlots[i].gameObject.SetActive(true);
            }
            else
            {
                UnitSlot slot = Instantiate(unitSlotPrefab, contents.transform).GetComponent<UnitSlot>();
                slot.SetStaff(DimensionManager.instance.dimensions[dimension][i]);
                unitSlots.Add(slot);
            }
        }

        for (int i = DimensionManager.instance.dimensions[dimension].Count; i < unitSlots.Count; i++)
        {
            unitSlots[i].gameObject.SetActive(false);
        }
    }

    void SetButton()
    {
        button.image.sprite = SpriteIconManager.instance.GetDimensionIcon(dimension);
        button.GetComponentInChildren<TextMeshProUGUI>().text = Enum.GetName(typeof(Dimension),dimension);
    }

    void OnButton()
    {
        //TODO:: 버튼 클릭 시 차원 정보 오픈
    }
}

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
    public GameObject unitSlotPrefab;   //���ֽ���������
    public GameObject contents;          //������ �θ������Ʈ

    Dimension dimension;

    Button dimensionButton;
    List<UnitSlot> unitSlots;
    void Awake()
    {
        dimensionButton = GetComponentInChildren<Button>();
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

    public Dimension Dimension
    {
        get { return dimension; }
        set { dimension = value; }
    }

    void SetUnitList()
    {
        for (int i = 0; i < DimensionManager.instance.dimensions[dimension].Count; i++)
        {
            if (i < unitSlots.Count) //������ �����ִٸ�
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
        dimensionButton.image.sprite = SpriteIconManager.instance.GetDimensionIcon(dimension);
        dimensionButton.GetComponentInChildren<TextMeshProUGUI>().text = Enum.GetName(typeof(Dimension),dimension);
    }

    void OnButton()
    {
        //TODO:: ��ư Ŭ�� �� ���� ���� ����
    }
}

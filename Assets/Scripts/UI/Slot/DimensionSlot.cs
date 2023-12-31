using EnumManager;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DimensionSlot : MonoBehaviour
{
    //������ư 1��, ���ֽ��� ����Ʈ�� ����

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
        for (int i = 0; i < DimensionManager.instance.dimensionStaff[dimension].Count; i++)
        {
            if (i < unitSlots.Count) //������ �����ִٸ�
            {
                unitSlots[i].SetStaff(DimensionManager.instance.dimensionStaff[dimension][i]);
                unitSlots[i].gameObject.SetActive(true);
            }
            else
            {
                UnitSlot slot = Instantiate(unitSlotPrefab, contents.transform).GetComponent<UnitSlot>();
                slot.SetStaff(DimensionManager.instance.dimensionStaff[dimension][i]);
                unitSlots.Add(slot);
            }
        }

        for (int i = DimensionManager.instance.dimensionStaff[dimension].Count; i < unitSlots.Count; i++)
        {
            unitSlots[i].gameObject.SetActive(false);
        }
    }

    void SetButton()
    {
        dimensionButton.image.sprite = SpriteManager.instance.GetDimensionImage(dimension);
        dimensionButton.GetComponentInChildren<TextMeshProUGUI>().text = Enum.GetName(typeof(Dimension),dimension);
    }

    void OnButton()
    {
        //TODO:: ��ư Ŭ�� �� ���� ���� ����
    }
}

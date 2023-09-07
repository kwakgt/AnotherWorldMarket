using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
    Staff staff;
    Image faceImage;
    Image workImage;
    TextMeshProUGUI unitName;
    Slider workGauge;

    void Awake()
    {
        faceImage = transform.GetChild(0).GetComponent<Image>();
        workImage = transform.GetChild(1).GetComponent<Image>();
        unitName = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        workGauge = transform.GetChild(3).GetComponent<Slider>();
    }

    void Update()
    {

    }

    public void SetStaff(Staff _staff)
    {
        staff = _staff;
    }


}

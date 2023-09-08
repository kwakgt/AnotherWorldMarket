using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
    Staff staff;
    public Image faceImage;
    public Image workImage;
    public TextMeshProUGUI unitName;
    public Slider workGauge;

    void Awake()
    {
        faceImage = transform.GetChild(0).GetComponent<Image>();
        workImage = transform.GetChild(1).GetComponent<Image>();
        unitName = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        workGauge = transform.GetChild(3).GetComponent<Slider>();
    }

    void Update()
    {
        Display();
    }

    public void SetStaff(Staff _staff)
    {
        staff = _staff;
    }

    void Display()
    {
        if(staff != null)
        {
            //TODO:: ���̹���, �۾��̹��� �߰�
            workGauge.maxValue = staff.slider.maxValue;
            workGauge.value = staff.slider.value;
        }

        //staff == null �̸� StaffManagementSlot�� �ı��ȴ�.
    }
}
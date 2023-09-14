using EnumManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //얼굴이미지, 현재 작업중인 이미지, 이름텍스트, 작업게이지 각각 1개로 구성

    Staff staff;
    public Image faceImage;
    public Image workImage;
    public TextMeshProUGUI unitName;
    public Slider workGauge;

    Transform parent;
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
            //TODO:: 얼굴이미지 추가
            workGauge.maxValue = staff.slider.maxValue;
            workGauge.value = staff.slider.value;
            workImage.sprite = SpriteManager.instance.GetWorkImage(staff.GetWorkState(false));
        }

        //staff == null 이면 StaffManagementSlot이 파괴되거나 disable
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parent = transform.parent;

        //드래그 할 때 이미지가 맨 위에 보이도록 하기 위해
        transform.SetParent(transform.root); //부모를 루트객체로 변경
        transform.SetAsLastSibling();        //하이어라키 마지막으로 이동
    }

    public void OnDrag(PointerEventData eventData)
    {
        eventData.pointerDrag.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //현재 위치가 차원의 Rect안에 포함된다면 해당 차원 불러오기
        Dimension dimension;
        if(GameObject.Find("DimensionPanel").GetComponent<DimensionPanel>().DimensionContainsScreenPoint(eventData.position,out dimension))
        {
            staff.ShiftDimension(dimension);
        }

        transform.SetParent(parent);
    }
}

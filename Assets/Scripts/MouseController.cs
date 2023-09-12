using UnityEngine;
using EnumManager;

public class MouseController : MonoBehaviour
{
    //ScreenToWorldPoint용
    Camera cam;

    //마우스 클릭 감지용
    RaycastHit2D downHit;
    RaycastHit2D upHit;

    //패널 활성화/비활성화
    GameObject unitPanel;
    GameObject shelfPanel;
    GameObject warehousePanel;

    void Awake()
    {
        cam = GetComponent<Camera>();
        unitPanel = GameObject.Find("UnitPanel");
        shelfPanel = GameObject.Find("ShelfPanel");
        warehousePanel = GameObject.Find("WarehousePanel");
    }

    void Update()
    {
        Selection();
    }

    void Selection()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            downHit = Physics2D.Raycast(mousePosition, Vector2.zero);   //다운 시 Hit 저장
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            upHit = Physics2D.Raycast(mousePosition, Vector2.zero);     //업 시 Hit 저장


            if (downHit && upHit && downHit == upHit && !GameManager.instance.Equals(GameManager.GameMode.Builder))  //다운Hit, 업Hit 비교하여 같으면 클릭성공, 건설모드일 시 작동안함
            {
                if ((downHit.transform.CompareTag("Customer") || downHit.transform.CompareTag("Staff")) && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedUnit = downHit.transform.GetComponent<Unit>();
                    PanelSetActive(true, false, false);
                }
                else if (downHit.transform.CompareTag("Shelf") && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedShelf = downHit.transform.GetComponent<Shelf>();
                    PanelSetActive(false, true, false);
                }
                else if (downHit.transform.CompareTag("Warehouse") && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedWarehouse = downHit.transform.GetComponent<Warehouse>();
                    PanelSetActive(false, false, true);
                }
                else
                {
                    GameManager.instance.selectedUnit = null;
                    PanelSetActive(false, false, false);
                }
                //TODO::다른 태그 선택 클릭시 추가
            }
            else
            {
                GameManager.instance.selectedUnit = null;
                PanelSetActive(false, false, false);
            }
        }
    }
    
    void PanelSetActive(bool _customerPanel, bool _shelfPanel, bool _warehousePanel)
    {
        unitPanel.SetActive(_customerPanel);
        shelfPanel.SetActive(_shelfPanel);
        warehousePanel.SetActive(_warehousePanel);
    }
}

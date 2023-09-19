using UnityEngine;
using EnumManager;

public class MouseController : MonoBehaviour
{
    //ScreenToWorldPoint용
    Camera cam;

    //마우스 클릭 감지용
    RaycastHit2D downHit;
    RaycastHit2D upHit;

    void Awake()
    {
        cam = GetComponent<Camera>();
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


            if (downHit && upHit && downHit == upHit && !GameManager.instance.Equals(GameMode.Building))  //다운Hit, 업Hit 비교하여 같으면 클릭성공, 건설모드일 시 작동안함
            {
                if ((downHit.transform.CompareTag("Customer") || downHit.transform.CompareTag("Staff")) && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedUnit = downHit.transform.GetComponent<Unit>();
                    UIManager.instance.ExecuteSelectedPanelOnOff(PanelName.UnitPanel);
                }
                else if (downHit.transform.CompareTag("Shelf") && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedShelf = downHit.transform.GetComponent<Shelf>();
                    UIManager.instance.ExecuteSelectedPanelOnOff(PanelName.ShelfPanel);
                }
                else if (downHit.transform.CompareTag("Warehouse") && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedWarehouse = downHit.transform.GetComponent<Warehouse>();
                    UIManager.instance.ExecuteSelectedPanelOnOff(PanelName.WarehousePanel);
                }
                else if (downHit.transform.CompareTag("Factory") && UIManager.instance.IsAllOff)
                {
                    GameManager.instance.selectedFactory = downHit.transform.GetComponent<Factory>();
                    UIManager.instance.ExecuteSelectedPanelOnOff(PanelName.FactoryPanel);
                }
                else
                {
                    GameManager.instance.selectedUnit = null;
                    GameManager.instance.selectedShelf = null;
                    GameManager.instance.selectedWarehouse = null;
                    GameManager.instance.selectedFactory = null;
                    UIManager.instance.ExecuteSelectedPanelOnOff(PanelName.Off);
                }
                //TODO::다른 태그 선택 클릭시 추가
            }
            else
            {
                GameManager.instance.selectedUnit = null;
                GameManager.instance.selectedShelf = null;
                GameManager.instance.selectedWarehouse = null;
                GameManager.instance.selectedFactory = null;
            }
        }
    }
}

using UnityEngine;


public class MouseController : MonoBehaviour
{
    //ScreenToWorldPoint용
    Camera cam;

    //마우스 클릭 감지용
    RaycastHit2D downHit;
    RaycastHit2D upHit;

    //패널 활성화/비활성화
    GameObject customerPanel;
    GameObject shelfPanel;
    GameObject warehousePanel;
    void Awake()
    {
        cam = GetComponent<Camera>();
        customerPanel = GameObject.Find("CustomerPanel");
        shelfPanel = GameObject.Find("ShelfPanel");
        warehousePanel = GameObject.Find("WarehousePanel");
    }

    void Update()
    {
        Selection();
    }

    void Selection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            downHit = Physics2D.Raycast(mousePosition, Vector2.zero);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            upHit = Physics2D.Raycast(mousePosition, Vector2.zero);
        }

        if (downHit && upHit && downHit == upHit)
        {
            if (downHit.transform.CompareTag("Customer"))
            {
                GameManager.instance.selectedUnit = downHit.transform.GetComponent<Unit>();
                customerPanel.SetActive(true);
            }
            else if(downHit.transform.CompareTag("Shelf"))
            {
                GameManager.instance.selectedShelf = downHit.transform.GetComponent<Shelf>();
                shelfPanel.SetActive(true);
            }
            else if (downHit.transform.CompareTag("Warehouse"))
            {
                GameManager.instance.selectedWarehouse = downHit.transform.GetComponent<Warehouse>();
                warehousePanel.SetActive(true);
            }
            //TODO::다른 태그 선택 클릭시 추가
        }
        else
        {
            GameManager.instance.selectedUnit = null;
            customerPanel.SetActive(false);
            shelfPanel.SetActive(false);
            warehousePanel.SetActive(false);
        }
    }
}

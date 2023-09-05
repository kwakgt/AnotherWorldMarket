using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;



public class Unit : MonoBehaviour //IPointerClickHandler //UI가 아니면 카메라에 Physics2DRaycater 컴포넌트 필요
{
    public Vector2 target;             //이동목표
    Vector2[] path;             //찾은 경로
    int pathIndex;              //경로 인덱스, 경로 그리기용
    protected Shelf shelf;                //찾은 판매대

    float speed = 5;            //속도
    public int gridIndex { get; private set; }              //현재 속해있는 그리드 인덱스
    public int invenSizeAvailable { get; private set; } = ((int)consumables.PlasticBag);  //사용가능한인벤토리


    protected Item[] inventory;  //인벤토리
    int maxInvenSize = 12;
    protected Vector2 respawn;            //탄생,소멸위치


    TextMeshProUGUI nameText;   //자식인덱스 0
    protected TextMeshProUGUI priceText;  //자식인덱스 1;
    Slider slider;              //자식인덱스 2;

    protected virtual void Awake()
    {
        inventory = new Item[maxInvenSize];
        nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        slider = transform.GetChild(0).GetChild(2).GetComponent<Slider>();
    }
    protected virtual void Start()
    {
        gridIndex = Nodefinding.instance.GetGridIndex(transform.position);
        respawn = UnitManager.instance.GetRespawn();    //test
        StartCoroutine(RefreshPath());

        //Test
        DataManager.Instance.UpdateData(gameObject);
    }

    IEnumerator RefreshPath()
    {
        yield return null;      //GoMarket()함수가 Start()에서 처음 실행하는데 다른 클래스 초기화보다 먼저 시작하면 NULL참조에러 발생하므로 한 사이클 돌리고 실행
        GoMarket();
        Vector2 targetPositionOld = target + Vector2.up; // 처음에 target.position에 != 보장

        while (true)
        {
            if (targetPositionOld != target)    //타겟이 바뀌면 바로 수행
            {
                targetPositionOld = target;     //OLD 타겟포지션과 타겟포지션을 같게 만들어 한번만 수행되게 실행, 타겟이 변경되면 다시 수행

                path = Pathfinding.RequestPath(transform.position, target, gridIndex);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            pathIndex = 0;    //path 인덱스
            Vector2 currentWaypoint = path[0];

            while (true)
            {
                if ((Vector2)transform.position == currentWaypoint) //목표위치에 도착완료하면 다음 웨이포인트로 이동
                {
                    pathIndex++;
                    if (pathIndex >= path.Length)     //path 인덱스가 path 길이만큼 오면 도착
                    {
                        break;
                    }
                    currentWaypoint = path[pathIndex];
                }

                //Vector2.MoveTowards(current,target,MaxDistanceDelta) : current에서 target까지 MaxDistanceDelta만큼 움직인 좌표
                transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }

            //현재 target에 도착한 상태, 아래에 이 후 행동 지정
            if (tag.Equals("Customer"))
                yield return StartCoroutine("CustomerRoutine");
            else
                yield return StartCoroutine("StaffRoutine");
        }
    }
    IEnumerator Waiting(float waitTime)
    {
        slider.gameObject.SetActive(true);
        slider.maxValue = waitTime;
        while (slider.value < slider.maxValue)
        {
            slider.value += Time.deltaTime;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        slider.value = 0;
    }
    protected void GoMarket(int index = -1)
    {
        float time = 0f;
        do
        {
            shelf = ShelfManager.instance.RequestRandomShelf();
            if (shelf == null)
            {
                Debug.Log("마켓에 아무것도 없네");
                time += Time.deltaTime;
                if (time > 30f) Destroy(gameObject);    //판매대가 없으면 고객 30초 후에 유닛 삭제
            }
            else
                target = shelf.GetFrontPosition(index);
        } while ((Vector2)transform.position == target);    //내위치가 타겟위치와 같으면 타겟 재세팅
    }

    protected void GoPortal()
    {
        if (gridIndex == 0)
            target = GameManager.instance.portals[0].GetFrontPosition();
        else if (gridIndex == 1)
            target = GameManager.instance.portals[1].GetFrontPosition();
    }

    protected void Teleport()
    {
        if(gridIndex == 0)
        {
            if(GameManager.instance.portals[0].FindIndexOfFrontPosition(transform.position) > -1)
            {
                transform.position = GameManager.instance.portals[1].GetFrontPosition();
                gridIndex = 1;
            }
        }
        else if(gridIndex == 1)
        {
            if (GameManager.instance.portals[1].FindIndexOfFrontPosition(transform.position) > -1)
            {
                transform.position = GameManager.instance.portals[0].GetFrontPosition();
                gridIndex = 0;
            }
        }
    }

    protected void GoHome()
    {
        target = respawn;
    }

    protected int FindItemIndexInInventory(Item itemToFind)
    {
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(itemToFind))
                return i;
        }
        return -1;
    }

    public Item GetItemInInven(int  index)
    {
        return inventory[index];
    }

    protected bool IsInventoryFull()
    {
        int count = 0;
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null)
                ++count;
        }

        if (count >= invenSizeAvailable)
            return true;
        else
            return false;
    }

    protected bool IsInventoryEmpty()
    {
        int count = 0;
        for (int i = 0; i < invenSizeAvailable; i++) 
        {
            if (inventory[i] == null)
                ++count;
        }

        if (count >= invenSizeAvailable)
            return true;
        else
            return false;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = pathIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

                if (i == pathIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerClick = this.gameObject)
        {
            GameManager.instance.selectedUnit = this;
        }

        Debug.Log(GameManager.instance.selectedUnit);
    }*/

    public enum consumables { TwoHands = 2, PlasticBag = 4, Basket = 8, Cart = 12 }
}
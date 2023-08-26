using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour //IPointerClickHandler //UI가 아니면 카메라에 Physics2DRaycater 컴포넌트 필요
{
    public      Vector2 target;             //이동목표
    protected   Shelf shelf;                //찾은 판매대
                Vector2[] path;             //찾은 경로
                int pathIndex;              //경로 인덱스, 경로 그리기용

    
    protected   Vector2 respawn;            //탄생,소멸위치
    public      int buyCount = 5;           //구매횟수
    public      int money = 1000;           //소지금
                float speed = 5;            //속도

    
    protected   Type type;                  //유형
                int invenSize = 12;
    public      int invenSizeAvailable { get; private set; } = ((int)consumables.PlasticBag);  //사용가능한인벤토리
    public      Item[] inventory { get; private set; }  //인벤토리
    protected   Heap<Index> invenIndex;     //Index.value가 inventory 인덱스

                TextMeshProUGUI nameText;   //자식인덱스 0
    protected   TextMeshProUGUI priceText;  //자식인덱스 1;
                Slider slider;              //자식인덱스 2;

    protected virtual void Awake()
    {
        inventory = new Item[invenSize];
        nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        slider = transform.GetChild(0).GetChild(2).GetComponent<Slider>();
        FullChagingHeapIndex();
    }
    protected virtual void Start()
    {
        respawn = UnitManager.instance.GetRespawn();    //test
        StartCoroutine(RefreshPath());
    }

    void Update()
    {

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

                path = Pathfinding.RequestPath(transform.position, target);
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
            if (type == Type.Customer)
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
        do
        {
            shelf = ShelfManager.instance.RequestRandomShelf();
            if (shelf == null)
                Debug.Log("마켓에 아무것도 없네");
            else
                target = shelf.GetShelfFrontPosition(index);
        } while ((Vector2)transform.position == target);    //내위치가 타겟위치와 같으면 타겟 재세팅
    }

    public void GoHome()
    {
        target = respawn;
    }

    void FullChagingHeapIndex() //사용할 인벤토리 인덱스 넣기
    {
        invenIndex = new Heap<Index>(invenSize);
        for (int i = 0; i < invenSize; i++)
        {
            Index value = new Index(i);
            invenIndex.Add(value);
        }
    }

    

    protected int FindKeyByValue(Item _item)
    {
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(_item))
                return i;
        }
        return -1;
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
    public enum Type { Customer, Staff}
}
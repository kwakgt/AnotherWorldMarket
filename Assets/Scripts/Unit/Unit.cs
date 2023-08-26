using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour //IPointerClickHandler //UI가 아니면 카메라에 Physics2DRaycater 컴포넌트 필요
{
    public Vector2 target;      //이동목표
    Shelf shelf;                //찾은 판매대
    Vector2[] path;             //찾은 경로
    int pathIndex;              //경로 인덱스, 경로 그리기용
    protected Vector2 respawn;            //탄생,소멸위치

    int buyCount = 5;           //구매횟수
    int money = 1000;           //소지금
    int maxWaitTime = 3;        //대기시간
    public float speed = 5;     //속도
   
    int invenSize = 12;
    public int invenSizeAvailable { get; private set; } = ((int)consumables.PlasticBag);  //사용가능한인벤토리
    public Item[] inventory { get; private set; }          //인벤토리
    Heap<Index> invenIndex;     //Index.value가 inventory 인덱스

    TextMeshProUGUI nameText;   //자식인덱스 0
    TextMeshProUGUI priceText;  //자식인덱스 1;
    Slider slider;              //자식인덱스 2;

    void Awake()
    {
        respawn = UnitManager.instance.GetRespawn();    //test
        inventory = new Item[invenSize];
        nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        slider = transform.GetChild(0).GetChild(2).GetComponent<Slider>();
        FullChagingHeapIndex();
    }
    void Start()
    { 
        StartCoroutine(RefreshPath());
        StartCoroutine(BuyingItem());
    }

    void Update()
    {
        nameText.text = "buyCount: " + buyCount + "\nmoney : " + money;     //test
    }

    IEnumerator RefreshPath()
    {
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
            if (shelf.FindItemInSlot(target) != null)           //매대에 물건이 있으면
                yield return StartCoroutine("BuyingItem");      //매대에 물건 구매

            if (money <= 0 || buyCount <= 0 || IsInventoryFull())   //돈, 구매횟수가 없거나 인벤토리가 가득 찼으면
                GoHome();                                           //집에 간다.
            else
                GoMarket();                                         //아니면 다른 매대 찾기

            if ((Vector2)transform.position == respawn)          //현재위치가 리스폰이라면 오브젝트 파괴
                Destroy(this.gameObject);
            yield return new WaitForSeconds(0.25f);             //과부하 방지용 대기시간
        }
    }

    IEnumerator BuyingItem()
    {
        if ((Vector2)transform.position == target)
        {
            Item shelfItem = shelf.FindItemInSlot(target);      //매대아이템 보기
            int shelfIndex = shelf.FindItemSlotIndex(target);   //매대아이템 슬롯 인덱스

            yield return StartCoroutine("Waiting");
            if (shelfItem == null) yield break;     //대기시간동안 물건이 다 팔릴수도 있으므로 한번 더 검사
            if (!FindKeyByValue(shelfItem).Equals(-1))                 //내 인벤토리에 아이템이 존재한다면, FindKeyByValue: 아이템을 인벤에서 찾을 수 없으면 인덱스 -1을 반환함
            {
                int key = FindKeyByValue(shelfItem);                    //인벤토리 키 찾기
                int amountToBuy = PurchaseForFitPrice(shelfItem);    //랜덤으로 아이템 구매량 정하기
                BuyItem(inventory[key], shelfItem, shelfIndex, amountToBuy);//구매량만큼 아이템 사서 인벤토리 아이템에 더하기
            }
            else                                                            //내 인벤토리에 아이템이 없다면
            {
                Item myItem = new Item(shelfItem);                          //아이템 새로 생성
                int amountToBuy = PurchaseForFitPrice(shelfItem);        //랜덤으로 아이템 구매량 정하기
                if (BuyItem(myItem, shelfItem, shelfIndex, amountToBuy))     //구매량만큼 아이템 사기
                    inventory[invenIndex.RemoveFirst().value] = myItem;   //인벤토리에 생성한 아이템 넣기
            }
            --buyCount;     //구매횟수 감소
            yield return new WaitForSeconds(0.25f);             //과부하 방지용 대기시간
        }
    }

    IEnumerator Waiting()
    {
        slider.gameObject.SetActive(true);
        slider.maxValue = maxWaitTime;
        while (slider.value < slider.maxValue)
        {
            slider.value += Time.deltaTime;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        slider.value = 0;
    }

    void GoMarket(int index = -1)
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

    bool BuyItem(Item myItem, Item shelfItem, int shelfIndex, int amountToBuy)
    {   
        //내 아이템에 구매량만큼 더하고 매대아이템에 구매량만큼 빼기
        money -= shelfItem.price * amountToBuy;                 //가격 계산
        if(money < 0)                                           //돈이 부족하면
        {
            money += shelfItem.price * amountToBuy;             //원상복구
            return false;
        }
        EnablePriceText("+" + (shelfItem.price * amountToBuy).ToString());
        myItem.PlusAmount(amountToBuy);                         //인벤토리의 아이템에 구매량만큼 추가
        shelfItem.MinusAmount(amountToBuy);                     //매대아이템에 구매량만큼 빼기
        if (shelfItem.amount <= 0)                              //매대아이템 양이 0 이하이면
            shelf.EmptyItemSlot(shelfIndex);                    //매대아이템 비우기

        Debug.Log("구매한 아이템 : " + myItem.name + " ,  구매량 : " + amountToBuy);
        return true;
    }

    void EnablePriceText(string text)
    {
        priceText.text = text;
        priceText.gameObject.SetActive(true);
    }

    int FindKeyByValue(Item _item)
    {
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(_item))
                return i;
        }
        return -1;
    }

    bool IsInventoryFull()
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
    
    int PurchaseForFitPrice(Item shelfItem)  //현재 소지금에 맞게 물건을 구입, 구매수량 정하는 함수
    {
        int amount = Random.Range(0, shelfItem.amount + 1);
        return Mathf.Clamp(amount, 0, money / shelfItem.price);
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
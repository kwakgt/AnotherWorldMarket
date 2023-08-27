using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Customer : Unit
{
    int amountOfPurchase = 3;
    int maxWaitTime = 3;        //대기시간

    Stopwatch sw = new Stopwatch();
    protected override void Awake()
    {
        base.Awake();
        type = Type.Customer;
        MarketManager.instance.customerCount++; //고객수++
    }

    protected override void Start()
    {
        sw.Start();
        base.Start();
    }

    IEnumerator CustomerRoutine()
    {
        if (shelf.FindItemInSlot(target) != null)           //매대에 물건이 있으면
            yield return StartCoroutine("BuyingItem");      //매대에 물건 구매

        if (money <= 0 || buyCount <= 0 || IsInventoryFull())   //돈, 구매횟수가 없거나 인벤토리가 가득 찼으면
            GoHome();                                           //집에 간다.
        else
            GoMarket();                                         //아니면 다른 매대 찾기

        if ((Vector2)transform.position == respawn)          //현재위치가 리스폰이라면 오브젝트 파괴
        {
            DestroyThis();
        }
        yield return new WaitForSeconds(0.25f);             //과부하 방지용 대기시간
    }

    IEnumerator BuyingItem()
    {
        if ((Vector2)transform.position == target)
        {
            Item shelfItem = shelf.FindItemInSlot(target);      //매대아이템 보기
            int shelfIndex = shelf.FindItemSlotIndex(target);   //매대아이템 슬롯 인덱스

            yield return StartCoroutine("Waiting", maxWaitTime);
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


    bool BuyItem(Item myItem, Item shelfItem, int shelfIndex, int amountToBuy)
    {
        //내 아이템에 구매량만큼 더하고 매대아이템에 구매량만큼 빼기
        money -= shelfItem.price * amountToBuy;                 //가격 계산
        if (money < 0)                                           //돈이 부족하면
        {
            money += shelfItem.price * amountToBuy;             //원상복구
            return false;
        }
        EnablePriceText(shelfItem.price * amountToBuy);
        MarketManager.instance.totalMoney += shelfItem.price * amountToBuy; //마켓매니저 소지금+
        myItem.PlusAmount(amountToBuy);                         //인벤토리의 아이템에 구매량만큼 추가
        shelfItem.MinusAmount(amountToBuy);                     //매대아이템에 구매량만큼 빼기
        if (shelfItem.amount <= 0)                              //매대아이템 양이 0 이하이면
            shelf.EmptyItemSlot(shelfIndex);                    //매대아이템 비우기

        return true;
    }

    void EnablePriceText(int price)
    {
        string text = "+" + price;
        priceText.text = text;
        priceText.gameObject.SetActive(true);
    }

    int PurchaseForFitPrice(Item shelfItem)  //현재 소지금에 맞게 물건을 구입, 구매수량 정하는 함수
    {
        int amount = Random.Range(1, amountOfPurchase + 1);
        return Mathf.Clamp(amount, 0, money / shelfItem.price);
    }

    void DestroyThis()
    {
        sw.Stop();
        MarketManager.instance.customerTotalCycleTime += sw.Elapsed.Seconds; //사이클 시간+
        MarketManager.instance.customerCount--;
        MarketManager.instance.deadCustomer++;
        Destroy(gameObject);
    }
}

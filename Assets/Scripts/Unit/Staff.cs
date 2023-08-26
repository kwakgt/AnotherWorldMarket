using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Unit
{
    int amountOfCarrying = 20;  //한번에 운반가능한 최대개수
    int workTime = 5;

    Warehouse warehouse;
    WorkType workType;          //상태 플래그
    List<CheckingItem> checkingItems = new List<CheckingItem>();    //확인품목리스트
    
    int workCount;                  //작업의 진행횟수,작업인덱스,인벤인덱스
    float checkingTime = 0.5f;      //확인시간
    protected override void Awake()
    {
        base.Awake();
        type = Type.Staff;
        workType = WorkType.Checking;
    }

    protected override void Start()
    {
        base.Start();
    }

    IEnumerator StaffRoutine()
    {

        if (workType == WorkType.Checking)          //매대에 아이템 확인
        {
            if (shelf.FindItemInSlot(target) != null)
            {
                yield return StartCoroutine("Checking");
                if (workCount >= invenSizeAvailable || checkingItems.Count >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 창고로 가기(인벤토리 가득참)
                {
                    workType = WorkType.Finding;
                    workCount = 0;
                    GoWarehouse();
                }
                else
                {
                    GoMarket();
                }
            }
        }
        else if(workType == WorkType.Finding)       //창고에서 아이템 찾기
        {
            yield return StartCoroutine("Finding");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 가기(인벤토리 가득참)
            {
                workType = WorkType.Carrying;
                workCount = 0;
                target = checkingItems[workCount].shelf.GetShelfFrontPosition(checkingItems[workCount].frontIndex); //첫번째 확인리스트의 판매대로 타겟지정
                Debug.Log("캐리!");
            }
            else
            {
                GoWarehouse();
            }
        }
        else if (workType == WorkType.Carrying)     //찾은 물건들 매대로 물건 옮기기
        {
            yield return StartCoroutine("Carrying");

        }
 
        //TODO:: 작업에 따라 추가
    }


    IEnumerator Checking()
    {
        yield return StartCoroutine("Waiting", checkingTime);
        Item shelfItem = shelf.FindItemInSlot(target);                                          //옮길 아이템
        int shelfIndex = shelf.FindItemSlotIndex(target);                                       //아이템 인덱스
        int amountCarring = Mathf.Clamp(amountOfCarrying, 1, shelfItem.amountOfShelf);          //옮길 수량
        if (shelfItem == null)                                                                  //아이템이 없으면 다른 매대 찾기
        {
            GoMarket();
            yield break;
        }
        
        checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem, amountCarring));       //확인리스트에 아이템 저장
        ++workCount;
    }

    IEnumerator Finding()
    {
        yield return StartCoroutine("Waiting", workTime);
        Item itemToFind = checkingItems[workCount].shlefItem;           //찾을 아이템
        int itemIndex = warehouse.FindItemIndexInInventory(itemToFind); //찾은 아이템인덱스
        if (itemIndex < 0) yield break;
        Item itemFound = warehouse.inventory[itemIndex];                //찾은 아이템

        if (itemFound != null)
        {
            Debug.Log("아이템 넣기");
            int amount = PutForFitAmountOfCarrying(itemFound);        //넣을 아이템 양
            if (warehouse.FindItemInWarehouse(itemFound))               //찾은 아이템이 창고에 있다면
            {
                PutItemInInventory(itemFound, itemIndex, amount); //인벤토리로 옮기기
            }
        }
        ++workCount;
    }

    IEnumerator Carrying()
    {
        yield return StartCoroutine("Waiting", workTime);
        Item shelfItem = checkingItems[workCount].shelf.ItemSlot[checkingItems[workCount].frontIndex];  //판매대 아이템
        if (shelfItem != null && shelfItem == inventory[workCount])   //판매대 아이템과 내 인벤토리 아이템이 같으면
        {
            int amount = EjectForFitAmountOfCarrying(inventory[workCount], shelfItem);
            EjectItemInInventory(shelfItem, amount);
        }
        ++workCount;
    }

    int PutForFitAmountOfCarrying(Item itemFound)
    {
        int maxAmountCarring = Mathf.Min(checkingItems[workCount].amountCarring, itemFound.amount);    //확인한 양과 창고 아이템 양중에 작은 값이 운반할 MAX양
        return Mathf.Clamp(amountOfCarrying, 1, maxAmountCarring);                                         
    }

    int EjectForFitAmountOfCarrying(Item myItem, Item fromItem)
    {
        int maxAmountCarring = Mathf.Min(fromItem.amountOfShelf - fromItem.amount, amountOfCarrying);   //판매대에 넣을수 있는 양과 내 운반량중에 작은 값이 운반할 MAX양
        return Mathf.Clamp(myItem.amount, 1, maxAmountCarring);
    }

    void PutItemInInventory(Item itemFound, int index, int amount)
    {
        inventory[workCount] = new Item(itemFound);
        inventory[workCount].PlusAmount(amount);
        itemFound.MinusAmount(amount);
        if(itemFound.amount.Equals(0))
        {
            warehouse.inventory[index] = null;
        }
    }

    void EjectItemInInventory(Item shelfItem, int amount)
    {
        shelfItem.PlusAmount(amount);
        inventory[workCount].MinusAmount(amount);
        if(inventory[workCount].amount.Equals(0))
        {
            inventory[workCount] = null;
        }
    }

    void GoWarehouse()
    {
        warehouse = WarehouseManager.instance.FindItemInWarehouseList(checkingItems[workCount].shlefItem);
        if (warehouse != null)
        {
            target = warehouse.GetWarehouseFrontPosition(target);
            return;
        }
        else
        {
            ++workCount;
            GoWarehouse();
        }
    }

   

    public class CheckingItem
    {
        public Shelf shelf;
        public int frontIndex;
        public Item shlefItem;
        public int amountCarring;

        public CheckingItem(Shelf _shelf, int _frontIndex, Item _shlefItem, int _amountCarring)
        {
            shelf = _shelf;
            frontIndex = _frontIndex;
            shlefItem = _shlefItem;
            amountCarring = _amountCarring;
        }
    }

    enum WorkType { Checking, Finding, Carrying, Emptying }
}



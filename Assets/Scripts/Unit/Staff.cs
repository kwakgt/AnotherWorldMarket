using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Staff : Unit
{
    int amountOfCarrying = 20;  //한번에 운반가능한 최대개수
    int workTime = 1;

    Warehouse warehouse;
    WorkType workType;              //현재 작업상태
    WorkType nextWorkType;          //텔레포트 다음 작업상태
    List<CheckingItem> checkingItems = new List<CheckingItem>(); //아이템 재고 확인리스트
    
    int workCount;                  //작업의 진행횟수,작업인덱스,인벤인덱스
    float checkingTime = 0.5f;      //확인시간
    protected override void Awake()
    {
        base.Awake();
        workType = WorkType.Checking;
    }

    protected override void Start()
    {
        base.Start();
    }

    IEnumerator StaffRoutine()
    {
        //아이템 재고 확인중
        if (workType == WorkType.Checking)
        {
            yield return StartCoroutine("Checking");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 창고로 가기(인벤토리 가득참)
            {
                workType = WorkType.Teleporting;
                nextWorkType = WorkType.Finding;
                workCount = 0;
                GoPortal();
            }
            else
            {
                GoMarket();
            }

        }
        //창고에서 아이템 찾기
        else if (workType == WorkType.Finding)
        {
            yield return StartCoroutine("Finding");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 가기(인벤토리 가득참)
            {
                workType = WorkType.Teleporting;
                nextWorkType = WorkType.Carrying;
                workCount = 0;
                GoPortal();
            }
            else
            {
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
        }
        //창고에서 판매대로 아이템 옮기기
        else if (workType == WorkType.Carrying)
        {
            yield return StartCoroutine("Carrying");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 다시 창고로 가기
            {
                if (IsInventoryEmpty()) //만약 남은 아이템이 없고, 다시 판매대 확인하러 가기
                {
                    workType = WorkType.Checking;
                    workCount = 0;
                    checkingItems.Clear();
                    GoMarket();
                }
                else                    //만약 남은 아이템이 있다면, 창고로 가서 아이템 비우기
                {
                    workType = WorkType.Teleporting;
                    nextWorkType = WorkType.Emptying;
                    workCount = 0;
                    GoPortal(); 
                }
            }
            else
            {
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //확인리스트의 판매대로 타겟지정
            }

        }
        //남은 아이템 창고로 옮기기
        else if (workType == WorkType.Emptying)
        {
            yield return StartCoroutine("Emptying");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 확인하러 가기(인벤토리 비움)
            {
                workType = WorkType.Teleporting;
                nextWorkType = WorkType.Checking;
                workCount = 0;
                checkingItems.Clear();
                GoPortal();
            }
            else
            {
                //yield return StartCoroutine("Emptying"); 여기에 있으면 아래 workCount에서 인덱스 에러 발생함
                target = warehouse.GetRandomWarehouseFrontPosition(target); //확인리스트의 판매대로 타겟지정
            }

        }
        //포탈 텔레포트
        else if(workType == WorkType.Teleporting)
        {
            yield return null;
            Teleport();
            if(nextWorkType == WorkType.Checking)
            {
                workType = nextWorkType;
                GoMarket();
            }
            else if(nextWorkType == WorkType.Finding)
            {
                workType = nextWorkType;
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
            else if(nextWorkType == WorkType.Carrying)
            {
                workType = nextWorkType;
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //첫번째 확인리스트의 판매대로 타겟지정
            }
            else if(nextWorkType == WorkType.Emptying)
            {
                workType = nextWorkType;
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
        }
 
        //TODO:: 작업에 따라 추가
    }


    IEnumerator Checking()  //아이템 재고 확인
    {
        yield return StartCoroutine("Waiting", checkingTime);
        int shelfIndex = shelf.FindIndexOfFrontPosition(target);      //옮길 아이템 인덱스
        if(shelfIndex == -1) shelfIndex = workCount;            //만약 -1이면 아무 인덱스 넣기
        Item shelfItem = shelf.GetItemInInven(shelfIndex);           //옮길 아이템
        if (shelfItem == null)                                  //아이템이 없으면 다른 매대 찾기
        {
            //TODO:: 랜덤아이템이 아닌 창고에 있는 아이템으로 변경
            shelfItem = ItemManager.instance.GetRandomItem();   //아이템이 없다면 새아이템으로 채워넣기
        }
        int amountCarring = Mathf.Clamp(shelfItem.amountOfShelf - shelfItem.amount, 0, amountOfCarrying);   //옮길 수량
        checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem, amountCarring));       //확인리스트에 아이템 저장
        ++workCount;
    }

    IEnumerator Finding() //아이템 찾기(창고에서 내 인벤토리로 아이템 옮기기)
    {
        yield return StartCoroutine("Waiting", workTime);
        Item itemToFind = checkingItems[workCount].shlefItem;           //찾을 아이템
        int itemIndex = warehouse.FindItemIndexInInventory(itemToFind); //찾은 아이템인덱스
        if (itemIndex > -1)                                             //찾을 아이템이 존재한다면,1중
        {
            Item itemFound = warehouse.GetItemInInven(itemIndex);            //찾은 아이템

            if (itemFound != null) //2중
            {
                int maxAmountCarring = Mathf.Min(amountOfCarrying, itemFound.amount);
                int amount = Mathf.Clamp(checkingItems[workCount].amountCarring, 0, maxAmountCarring);
                if (warehouse.FindItemIndexInInventory(itemFound) > -1)               //찾은 아이템이 창고에 있다면, 3중
                {
                    PutItemInInventory(itemFound, itemIndex, amount);   //인벤토리로 옮기기
                }
            }
        }
        ++workCount;
    }

    IEnumerator Carrying() //아이템 운반(내 인벤토리에서 판매대로 아이템 옮기기)
    {
        yield return StartCoroutine("Waiting", workTime);
        Item shelfItem = checkingItems[workCount].shelf.GetItemInInven(checkingItems[workCount].frontIndex);  //판매대 아이템
        if (shelfItem != null && shelfItem.Equals(inventory[workCount]))   //판매대 아이템과 내 인벤토리 아이템이 같으면
        {
            int maxAmountCarring = Mathf.Min(shelfItem.amountOfShelf - shelfItem.amount, amountOfCarrying);   //판매대에 넣을수 있는 양과 내 운반량중에 작은 값이 운반할 MAX양
            int amount = Mathf.Clamp(inventory[workCount].amount, 0, maxAmountCarring);
            EjectItemInInventory(shelfItem, amount);
        }
        else if(shelfItem == null)  //판매대에 아이템이 없다면
        {
            if (inventory[workCount] != null)
            {
                Item newItem = new Item(inventory[workCount]);              //인벤 아이템을 복사해서
                EjectItemInInventory(newItem, inventory[workCount].amount); //전부 판매대로 옮기기
                checkingItems[workCount].shelf.PutItemInInven(checkingItems[workCount].frontIndex, newItem);    //판매대에 아이템 넣기
            }
            else
            {
                Debug.Log("창고에 " + checkingItems[workCount].shlefItem.name + "가 없네");
            }
        }
        ++workCount;
    }

    IEnumerator Emptying()  //남은 아이템 다시 창고로 옮기기
    {
        if (inventory[workCount] != null)
        {
            yield return StartCoroutine("Waiting", workTime);
            int itemIndex = warehouse.FindItemIndexInInventory(inventory[workCount]);
            if(itemIndex > -1) //창고에 같은 아이템이 존재하면
            {
                EjectItemInInventory(warehouse.GetItemInInven(itemIndex), inventory[workCount].amount);   //창고에 아이템 넣기
            }
            else //창고에 같은 아이템이 없다면
            {
                int index = warehouse.FirstEmptyIndexInInventory(); //빈칸 인덱스 찾기
                if (index > -1)       //빈칸이 있다면
                {
                    Item newItem = new Item(inventory[workCount]);
                    EjectItemInInventory(newItem, inventory[workCount].amount); //빈공간에 아이템 넣기
                    warehouse.PutItemInInven(index, newItem);
                }
                else
                    Debug.Log("창고가 꽉찼다");
            }
        }
        ++workCount;
    }

    void PutItemInInventory(Item itemFound, int index, int amount)
    {
        inventory[workCount] = new Item(itemFound);
        if(itemFound.MinusAmount(amount))
            inventory[workCount].PlusAmount(amount);
        if(itemFound.amount.Equals(0))
        {
            warehouse.EmptyInventory(index);
        }
    }

    void EjectItemInInventory(Item ejectedItem, int amount)
    {
        if(inventory[workCount].MinusAmount(amount))
            ejectedItem.PlusAmount(amount);
        if(inventory[workCount].amount.Equals(0))
        {
            inventory[workCount] = null;
        }
    }

    void GoWarehouse(Item itemToFind)
    {
        warehouse = WarehouseManager.instance.FindItemInWarehouseList(itemToFind);
        if (warehouse != null)
        {
            target = warehouse.GetRandomWarehouseFrontPosition(target); //아이템이 있는 창고가 있다면 창고로 가기
            return;
        }
        else
        {
            warehouse = WarehouseManager.instance.RequestRandomWarehouse();
            target = warehouse.GetRandomWarehouseFrontPosition(target); //없으면 랜덤창고로 가기
        }
    }

   

    //재고 확인 구조체
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

    enum WorkType { Checking, Finding, Carrying, Emptying, Teleporting }
}



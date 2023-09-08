using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;

public class Staff : Unit
{
    //command와 workType 변수로 상태 패턴 구현
    WorkType workType;              //현재 작업 상태
    WorkType nextWorkType;          //텔레포트 다음 작업 상태(창고를 가기 위해 포탈을 타야하므로 텔레포트 후 다음 작업을 저장해야된다)
    WorkType command;               //현재 명령 상태 (명령 수행 전에 아이템을 비우기 위해 Work.Emptying을 수행 후 실제로 받은 명령 저장ㅁ)
    WorkType receivedCommand;       //실제로 받은 명령(Work.Emptying을 수행해야하므로 실제 받은 명령 따로 저장해야된다.)
    
    Warehouse warehouse;
    List<CheckingItem> checkingItems = new List<CheckingItem>(); //아이템 재고 확인리스트

    public int uniIndex { get; private set; }   //고유번호
    int workCount;                              //작업의 진행횟수,작업인덱스,인벤인덱스
    float checkingTime = 0.5f;                  //확인시간
    protected override void Awake()
    {
        base.Awake();
        type = UnitType.Staff;
        command = WorkType.Carrying;
        workType = WorkType.Checking;

    }

    protected override void Start()
    {
        base.Start();
        uniIndex = UnitManager.instance.GetUniqueIndex();
        UnitManager.instance.AddStaff(this);
    }

    void WorkStateMachine(WorkType _command, WorkType _workType, WorkType _receivedCommand, WorkType _nextWorkType)
    {
        command = _command;
        workType = _workType;
        receivedCommand = _receivedCommand;
        nextWorkType = _nextWorkType;
    }

    public void ReceiveCommander(WorkType type) //버튼사용
    {
        if (command == type) return; //명령이 현재 명령과 같으면 무시

        workCount = 0;  //명령이 변경되면 무조건 0으로 초기화;
        if (gridIndex == 0)
        {
            //TODO:: 무조건 창고로 가기
            command = WorkType.Emptying;
            receivedCommand = type;
            workType = WorkType.Teleporting;
            nextWorkType = WorkType.Emptying;
            GoPortal();
        }
        else if (gridIndex == 1)
        {
            //TODO:: 아이템이 있으면 창고, 없으면 외부포탈
            if(IsInventoryEmpty())
            {
                command = receivedCommand;
                workType = receivedCommand;
                nextWorkType = receivedCommand;
                workCount = 0;
                checkingItems.Clear();
                GoExternalPortal();
            }
            else
            {
                workType = WorkType.Emptying;
            }
        }
    }

    IEnumerator StaffRoutine()
    {
        //처음 명령 받으면 수행, 아이템 비우기
        if (command == WorkType.Emptying)
        {
            yield return StartCoroutine(EmptyingRoutine());
        }
        //운반 명령, 유닛이 생성되면 시작하는 기본명령
        else if (command == WorkType.Carrying)
        {
            yield return StartCoroutine(CarryingRoutine());
        }
        else if (command == WorkType.Hunting)
        {
            
        }

        //TODO:: 작업에 따라 추가
    }
    IEnumerator EmptyingRoutine()
    {
        if (workType == WorkType.Teleporting)
        {
            yield return null;
            Teleport();
            if (IsInventoryEmpty())
            {
                //TODO::외부터널 가기
                command = receivedCommand;
                workType = receivedCommand;
                nextWorkType = receivedCommand;
                workCount = 0;
                checkingItems.Clear();
                GoExternalPortal();
            }
            else
            {   //인벤에 아이템이 있다면 비우러 가기
                workType = nextWorkType;
                GoWarehouse(inventory[workCount]);
            }
        }
        else if (workType == WorkType.Emptying)
        {
            yield return StartCoroutine("Emptying");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 확인하러 가기(인벤토리 비움)
            {
                command = receivedCommand;
                workType = receivedCommand;
                nextWorkType = receivedCommand;
                workCount = 0;
                checkingItems.Clear();
                GoExternalPortal();
            }
            else
            {
                target = warehouse.GetRandomWarehouseFrontPosition(target); //현재 위치를 제외한 창고의 랜덤입구로 가기
            }

        }
    }

    IEnumerator CarryingRoutine()
    {
        //아이템 재고 확인중
        if (workType == WorkType.Checking)
        {
            yield return StartCoroutine("Checking");
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 창고로 가기(인벤토리 가득참)
            {
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Finding);
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
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Carrying);
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
                    WorkStateMachine(command, WorkType.Checking, command, WorkType.Checking);
                    workCount = 0;
                    checkingItems.Clear();
                    GoMarket();
                }
                else                    //만약 남은 아이템이 있다면, 창고로 가서 아이템 비우기
                {
                    WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Emptying);
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
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Checking);
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
        else if (workType == WorkType.Teleporting)
        {
            yield return null;
            Teleport();
            if (nextWorkType == WorkType.Checking)
            {
                WorkStateMachine(command, nextWorkType, command, nextWorkType);
                GoMarket();
            }
            else if (nextWorkType == WorkType.Finding)
            {
                WorkStateMachine(command, nextWorkType, command, nextWorkType);
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
            else if (nextWorkType == WorkType.Carrying)
            {
                WorkStateMachine(command, nextWorkType, command, nextWorkType);
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //첫번째 확인리스트의 판매대로 타겟지정
            }
            else if (nextWorkType == WorkType.Emptying)
            {
                WorkStateMachine(command, nextWorkType, command, nextWorkType);
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
        }
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
        int amountCarring = Mathf.Clamp(shelfItem.amountOfShelf - shelfItem.amount, 0, stat.GetWorkingAmount(command));   //옮길 수량
        checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem, amountCarring));       //확인리스트에 아이템 저장
        ++workCount;
    }

    IEnumerator Finding() //아이템 찾기(창고에서 내 인벤토리로 아이템 옮기기)
    {
        yield return StartCoroutine("Waiting", checkingTime);
        Item itemToFind = checkingItems[workCount].shlefItem;           //찾을 아이템
        int itemIndex = warehouse.FindItemIndexInInventory(itemToFind); //찾은 아이템인덱스
        if (itemIndex > -1)                                             //찾을 아이템이 존재한다면,1중
        {
            Item itemFound = warehouse.GetItemInInven(itemIndex);            //찾은 아이템

            if (itemFound != null) //2중
            {
                int maxAmountCarring = Mathf.Min(stat.GetWorkingAmount(command), itemFound.amount);
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
        yield return StartCoroutine("Waiting", stat.GetWorkingTime(command));
        Item shelfItem = checkingItems[workCount].shelf.GetItemInInven(checkingItems[workCount].frontIndex);  //판매대 아이템
        if (shelfItem != null && shelfItem.Equals(inventory[workCount]))   //판매대 아이템과 내 인벤토리 아이템이 같으면
        {
            int maxAmountCarring = Mathf.Min(shelfItem.amountOfShelf - shelfItem.amount, stat.GetWorkingAmount(command));   //판매대에 넣을수 있는 양과 내 운반량중에 작은 값이 운반할 MAX양
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
            yield return StartCoroutine("Waiting", checkingTime);
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

    //정렬용,버튼
    void CompareTo(Unit other, WorkType sort = WorkType.Emptying)
    {
        //TODO:: 스탯별로 정렬 
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

    
}



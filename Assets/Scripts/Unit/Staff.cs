using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;
using System;
using Random = UnityEngine.Random;


public class Staff : Unit
{
    //command와 workType 변수로 상태 패턴 구현
    WorkType workType;              //현재 작업 상태
    WorkType nextWorkType;          //텔레포트 다음 작업 상태(창고를 가기 위해 포탈을 타야하므로 텔레포트 후 다음 작업을 저장해야된다)
    WorkType command;               //현재 명령 상태 (명령 수행 전에 아이템을 비우기 위해 Work.Emptying을 수행 후 실제로 받은 명령 저장)
    WorkType receivedCommand;       //실제로 받은 명령(Work.Emptying을 수행해야하므로 실제 받은 명령 따로 저장해야된다.)

    Dimension dimension;

    Warehouse warehouse;
    List<CheckingItem> checkingItems = new List<CheckingItem>(); //아이템 재고 확인리스트

    public int uniIndex { get; private set; }   //고유번호
    int workCount;                              //작업의 진행횟수,작업인덱스,인벤인덱스
    float checkingTime = 0.5f;                  //확인시간

    //코루틴 STOP 용
    Coroutine running;
    Coroutine waiting;
    protected override void Awake()
    {
        base.Awake();
        type = UnitType.Staff;
        command = WorkType.Carrying;
        workType = WorkType.Checking;
        dimension = Dimension.Astaria;
    }

    protected override void Start()
    {
        base.Start();
        uniIndex = UnitManager.instance.GetUniqueIndex();
        UnitManager.instance.AddStaff(this);
        DimensionManager.instance.EnterDimension(dimension, this);
    }

    void WorkStateMachine(WorkType _command, WorkType _workType, WorkType _receivedCommand, WorkType _nextWorkType) //작업상태머신
    {
        command = _command;                     //현재명령
        workType = _workType;                   //현재작업
        receivedCommand = _receivedCommand;     //받은명령 - 처음 명령 받을 때만 사용한다(EmptyingRoutine용). 그외는 아무값이나 넣어도 된다.
        nextWorkType = _nextWorkType;           //다음작업 - 내부포탈 쓸 때만 사용한다. 그 외는 아무값이나 넣어도 된다.

        //WorkStateMachine이 다음작업으로 변경해주니 다음작업에 따라 목적지도 같이 변경해준다.
        workCount = 0;
        if(command == WorkType.Emptying)
        {
            if(workType == WorkType.Teleporting)
            {
                GoPortal();
            }
            else if(workType == WorkType.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        else if(command == WorkType.Carrying)
        {
            if(workType == WorkType.Checking)
            {
                GoMarket();
            }
            else if(workType == WorkType.Finding)
            {
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
            else if(workType == WorkType.Carrying)
            {
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //첫번째 확인리스트의 판매대로 타겟지정
            }
            else if(workType == WorkType.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
            else if(workType == WorkType.Teleporting)
            {
                GoPortal();
            }
        }
        else if (IsDimensionWork())
        {
            if(workType == command)
            {
                GoDimentionPortal(target);
            }
            else if(workType == WorkType.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        
    }

    public WorkType GetWorkState(bool commandOrWorktype) //현재 작업상태
    {
        if (commandOrWorktype) return command;
        else return workType;
    }

    public bool IsDimensionWork() //차원포탈을 통한 작업 : 벌목,채광,채집,사냥,낚시
    {
        return command switch
        {
            WorkType.Felling or WorkType.Mining or WorkType.Collecting or WorkType.Hunting or WorkType.Fishing => true,
            _ => false
        };
            
    }

    public void ReceiveCommand(WorkType type) //버튼사용
    {
        if (command == type || type == WorkType.Purchase) return; //명령이 현재 명령과 같으면 무시

        if (running != null) StopCoroutine(running);
        if (waiting != null) StopCoroutine(waiting);

        workCount = 0;  //명령이 변경되면 무조건 0으로 초기화;
        if (gridIndex == 0)
        {
            //무조건 창고로 가기(포탈 -> 창고)
            WorkStateMachine(WorkType.Emptying, WorkType.Teleporting, type, WorkType.Emptying);
        }
        else if (gridIndex == 1)
        {
            //TODO:: 아이템이 있으면 창고, 없으면 외부포탈
            if (IsInventoryEmpty())
            {
                if (type == WorkType.Carrying)
                {
                    WorkStateMachine(WorkType.Carrying, WorkType.Teleporting, type, WorkType.Checking);
                    checkingItems.Clear();
                }
                else
                {
                    WorkStateMachine(type, type, type, type);
                }

            }
            else
            {   //인벤에 아이템이 있다면 비우러 가기
                WorkStateMachine(type, WorkType.Emptying, type, nextWorkType);
            }
        }
    }

    Coroutine RunningCoroutine(IEnumerator routine) //코루틴 저장/실행/중지
    {
        if (running != null)
            StopCoroutine(running);
        running = StartCoroutine(routine);
        return running;
    }

    Coroutine WaitingCoroutine(IEnumerator routine) //코루틴 저장/실행/중지
    {
        if (waiting != null)
            StopCoroutine(waiting);
        slider.value = 0;
        waiting = StartCoroutine(routine);
        return waiting;
    }

    IEnumerator StaffRoutine()
    {
        //처음 명령 받으면 수행, 가지고 있는 아이템 비우기
        if (command == WorkType.Emptying)
        {
            yield return StartCoroutine(EmptyingRoutine());
        }
        //운반 명령, 유닛이 생성되면 시작하는 기본명령
        else if (command == WorkType.Carrying)
        {
            yield return StartCoroutine(CarryingRoutine());
        }
        //차원포탈 작업루틴 : 벌목,채광,채집,사냥,낚시
        else if (IsDimensionWork())
        {
            yield return StartCoroutine(DimentionPortalRoutine());
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
                if (receivedCommand == WorkType.Carrying)
                {
                    WorkStateMachine(WorkType.Carrying, WorkType.Teleporting, receivedCommand, WorkType.Checking);
                    checkingItems.Clear();
                }
                else
                {
                    WorkStateMachine(receivedCommand, receivedCommand, receivedCommand, receivedCommand);
                }

            }
            else
            {   //인벤에 아이템이 있다면 비우러 가기
                WorkStateMachine(receivedCommand, WorkType.Emptying, receivedCommand, nextWorkType);
            }
        }
    }

    IEnumerator DimentionPortalRoutine()
    {
        if (workType == command)
        {
            //TODO:: 하이드 - 디멘션입장 - 자원채취 - 창고로 옮기기
            yield return RunningCoroutine(DimensionWork());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, WorkType.Emptying, command, workType);
                workCount = 0;
            }
            else
            {
                GoDimentionPortal(target);
            }
        }
        else if (workType == WorkType.Emptying)
        {
            yield return RunningCoroutine(Emptying());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, command, receivedCommand, command);
                workCount = 0;
            }
            else
            {
                GoWarehouse(inventory[workCount]);
            }
        }
    }

    IEnumerator CarryingRoutine()
    {
        //아이템 재고 확인중
        if (workType == WorkType.Checking)
        {
            yield return RunningCoroutine(Checking());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 창고로 가기(인벤토리 가득참)
            {
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Finding);
            }
            else
            {
                GoMarket();
            }

        }
        //창고에서 아이템 찾기
        else if (workType == WorkType.Finding)
        {
            yield return RunningCoroutine(Finding());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 가기(인벤토리 가득참)
            {
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Carrying);
            }
            else
            {
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
        }
        //창고에서 판매대로 아이템 옮기기
        else if (workType == WorkType.Carrying)
        {
            yield return RunningCoroutine(Carrying());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 다시 창고로 가기
            {
                if (IsInventoryEmpty()) //만약 남은 아이템이 없고, 다시 판매대 확인하러 가기
                {
                    WorkStateMachine(command, WorkType.Checking, command, WorkType.Checking);
                    checkingItems.Clear();
                }
                else                    //만약 남은 아이템이 있다면, 창고로 가서 아이템 비우기
                {
                    WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Emptying);
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
            yield return RunningCoroutine(Emptying());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 확인하러 가기(인벤토리 비움)
            {
                WorkStateMachine(command, WorkType.Teleporting, command, WorkType.Checking);
                checkingItems.Clear();
            }
            else
            {
                //yield return StartCoroutine("Emptying"); 여기에 있으면 아래 workCount에서 인덱스 에러 발생함
                GoWarehouse(inventory[workCount]); //확인리스트의 판매대로 타겟지정
            }

        }
        //포탈 텔레포트
        else if (workType == WorkType.Teleporting)
        {
            yield return null;
            Teleport();
            WorkStateMachine(command, nextWorkType, command, nextWorkType);
        }
    }

    IEnumerator Checking()  //아이템 재고 확인
    {
        waiting = WaitingCoroutine(Waiting(checkingTime));
        yield return waiting;
        int shelfIndex = shelf.FindIndexOfFrontPosition(target);      //옮길 아이템 인덱스
        if(shelfIndex == -1) shelfIndex = Random.Range(0,shelf.maxInvenSize);            //만약 -1이면 아무 인덱스 넣기,확인중에 건물을 옮기면 -1이 나옴
        Item shelfItem = shelf.GetItemInInven(shelfIndex);           //옮길 아이템
        if (shelfItem == null)                                  //아이템이 없으면 다른 매대 찾기
        {
            shelfItem = WarehouseManager.instance.GetItemInRandomWarehouse();   //아이템이 없다면 새아이템으로 채워넣기
        }
        checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem));       //확인리스트에 아이템 저장
        ++workCount;
    }

    IEnumerator Finding() //아이템 찾기(창고에서 내 인벤토리로 아이템 옮기기)
    {
        yield return WaitingCoroutine(Waiting(checkingTime));
        int itemIndex = warehouse.FindItemIndexInInventory(checkingItems[workCount].shlefItem);
        if (itemIndex > -1)                                             //찾을 아이템이 존재한다면,1중
        {
            Item itemFound = warehouse.GetItemInInven(itemIndex);            //찾은 아이템

            if (itemFound != null) //2중
            {
                int amount = Mathf.Clamp(itemFound.amount, 0, stat.GetWorkingAmount(command));
                PutItemInInventory(itemFound, amount, itemIndex);   //인벤토리로 옮기기
            }
        }
        ++workCount;
    }

    IEnumerator Carrying() //아이템 운반(내 인벤토리에서 판매대로 아이템 옮기기)
    {
        yield return WaitingCoroutine(Waiting(stat.GetWorkingTime(command)));
        Item shelfItem = checkingItems[workCount].shelf.GetItemInInven(checkingItems[workCount].frontIndex);  //판매대 아이템
        if (shelfItem != null && shelfItem.Equals(inventory[workCount]))   //판매대 아이템과 내 인벤토리 아이템이 같으면
        {
            int amount = Mathf.Clamp(inventory[workCount].amount, 0, shelfItem.amountOfShelf - shelfItem.amount);
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
            yield return WaitingCoroutine(Waiting(checkingTime));
            int itemIndex = warehouse.FindItemIndexInInventory(inventory[workCount],true);
            if(itemIndex > -1) //창고에 같은 아이템이 존재하고, 아이템 최대량이 아니면
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

    IEnumerator DimensionWork() //벌목,채광,채집,사냥,낚시
    {
        yield return WaitingCoroutine(Waiting(stat.GetWorkingTime(command)));
        Item collect = DimensionManager.instance.GetItem(dimension, command);   //아이템 가져오기
        if (collect != null && collect.amount != 0)
        {
            int amount = Mathf.Clamp(collect.amount, 1, stat.GetWorkingAmount(command));
            PutItemInInventory(collect, amount);
        }
        ++workCount;
    }

    public void ShiftDimension(Dimension _dimension)
    {
        if (dimension == _dimension) return;
        DimensionManager.instance.ShiftDimension(dimension, _dimension, this);
        dimension = _dimension;
    }

    void PutItemInInventory(Item itemFound, int amount, int index = -1) //아이템을 내 인벤에 넣기, 창고가 아니면 index에 -1
    {
        inventory[workCount] = new Item(itemFound);
        if(itemFound.MinusAmount(amount))
            inventory[workCount].PlusAmount(amount);
        if (itemFound.amount == 0 && index != -1)
        {
            warehouse.EmptyInventory(index);
        }
    }

    void EjectItemInInventory(Item ejectedItem, int amount) //아이템을 내 인벤에서 꺼내기
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
        if(warehouse == null)
        {
            warehouse = WarehouseManager.instance.FindEmptyWarehouse(); //아이템이 없다면 빈창고로 가기
            if (warehouse == null)
                warehouse = WarehouseManager.instance.RequestRandomWarehouse(); //아이템이 없고, 빈창고도 없으면 그냥 아무창고에 가기
        }

        target = warehouse.GetRandomWarehouseFrontPosition(target);
    }

    void GoPortal()
    {
        if (gridIndex == 0)
            target = GameManager.instance.portals[0].GetFrontPosition();
        else if (gridIndex == 1)
            target = GameManager.instance.portals[1].GetFrontPosition();
    }

    void GoDimentionPortal(Vector2 _target)
    {
        do
        {
            target = GameManager.instance.portals[2].GetFrontPosition();
        } while (target == _target);
        
    }

    //재고 확인 구조체
    public class CheckingItem
    {
        public Shelf shelf;
        public int frontIndex;
        public Item shlefItem;

        public CheckingItem(Shelf _shelf, int _frontIndex, Item _shlefItem)
        {
            shelf = _shelf;
            frontIndex = _frontIndex;
            shlefItem = _shlefItem;
        }
    }
}



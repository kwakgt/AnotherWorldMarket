using EnumManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Staff : Unit
{
    //command와 workType 변수로 상태 패턴 구현
    StaffWork command;               //현재 명령 상태 (명령 수행 전에 아이템을 비우기 위해 command = Emptying을 수행 후 실제로 받은 명령 저장)
    StaffWork workType;              //현재 작업 상태
    StaffWork receivedCommand;       //실제로 받은 명령(command = Emptying을 수행해야하므로 실제 받은 명령 따로 저장해야된다.), 처음 명령을 받을 때만 사용된다.
    StaffWork nextWorkType;          //텔레포트 다음 작업 상태(창고를 가기 위해 포탈을 타야하므로 텔레포트 후 다음 작업을 저장해야된다), 텔레포트 할 때만 사용된다.

    public Dimension dimension { get; private set; }

    //창고
    Warehouse warehouse;
    List<CheckingItem> checkingItems = new List<CheckingItem>(); //아이템 재고 확인리스트
    //공장
    Factory factory;
    int factoryIndex;

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
        command = StaffWork.Carrying;
        workType = StaffWork.Checking;
        dimension = Dimension.Astaria;
    }

    protected override void Start()
    {
        base.Start();
        uniIndex = UnitManager.instance.GetUniqueIndex();
        UnitManager.instance.AddStaff(this);
        DimensionManager.instance.EnterDimension(dimension, this);
    }

    void WorkStateMachine(StaffWork _command, StaffWork _workType, StaffWork _receivedCommand, StaffWork _nextWorkType) //작업상태머신
    {
        command = _command;                     //현재명령
        workType = _workType;                   //현재작업
        receivedCommand = _receivedCommand;     //받은명령 - command가 StaffWork.Emptying 일 때만 사용, 그외는 아무값이나 넣어도 된다.
        nextWorkType = _nextWorkType;           //다음작업 - workType이 StaffWork.Teleporting 일 때만 사용, 그 외는 아무값이나 넣어도 된다.

        //WorkStateMachine이 다음작업으로 변경해주니 다음작업에 따라 목적지도 같이 변경해준다.(추가로 workCount 초기화)
        workCount = 0;
        if (command == StaffWork.Emptying)
        {
            if (workType == StaffWork.Teleporting)
            {
                GoPortal();
            }
            else if (workType == StaffWork.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        else if (command == StaffWork.Carrying)
        {
            if (workType == StaffWork.Checking)
            {
                GoMarket();
            }
            else if (workType == StaffWork.Finding)
            {
                GoWarehouse(checkingItems[workCount].checkedItem);
            }
            else if (workType == StaffWork.Carrying)
            {
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //첫번째 확인리스트의 판매대로 타겟지정
            }
            else if (workType == StaffWork.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
            else if (workType == StaffWork.Teleporting)
            {
                GoPortal();
            }
        }
        else if (command == StaffWork.Deliverying)
        {
            if (workType == StaffWork.Checking)
            {
                GoFactory(command);
            }
            else if (workType == StaffWork.Finding)
            {
                GoWarehouse(checkingItems[workCount].checkedItem);
            }
            else if (workType == StaffWork.Deliverying)
            {
                target = checkingItems[workCount].factory.GetFrontPosition();
            }
            else if (workType == StaffWork.Loading)
            {
                target = checkingItems[workCount].factory.GetRandomFrontPosition(transform.position);
            }
            else if (workType == StaffWork.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        else if (IsDimensionWork(command))
        {
            if (workType == command)
            {
                GoDimensionPortal(target);
            }
            else if (workType == StaffWork.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        else if (IsFactoryWork(command))
        {
            if(workType == command)
            {
                GoFactory(command);
            }
            else if( workType == StaffWork.Emptying)
            {
                GoWarehouse(inventory[workCount]);
            }
        }
        
    }

    public StaffWork GetWorkState(bool commandOrWorktype) //현재 작업상태
    {
        if (commandOrWorktype) return command;
        else return workType;
    }

    bool CancleCommand(StaffWork type)  //명령 취소 조건 함수
    {
        //받은 명령이 현재 명령이면 취소
        if (command == type)
        {
            Debug.Log("현재 명령을 수행 중 입니다.");
            return true;
        }
        //작업 스탯이 0이면 명령 수행 불가
        else if (stat.ReplaceFromWorkTypeToInt(type) == 0)
        {
            Debug.Log("해당 직원은 " + type + " 작업을 수행 할 수 없습니다.");
            return true;
        }
        else if (IsFactoryWork(type) && BuildingManager.instance.RequestEmptyFactory(type) == null)
        {
            Debug.Log("작업 할 수 있는 " + type + " 공장이 없습니다.");
            return true;
        }
        else return false;
        //TODO::명령 취소 조건 작성
    }

    public void ReceiveCommand(StaffWork type) //버튼사용
    {
        if (CancleCommand(type)) return; //명령 취소

        if (running != null) StopCoroutine(running);
        if (waiting != null) StopCoroutine(waiting);
        
        //현재 명령이 공장 명령이면 다른 명령 받을 시 공장에서 나가기
        if(IsFactoryWork(command))  
            factory.ExitFactory(factoryIndex);

        workCount = 0;  //명령이 변경되면 무조건 0으로 초기화;
        if (gridIndex == 0)
        {
            //무조건 창고로 가기(마켓 -> 포탈 -> 창고)
            WorkStateMachine(StaffWork.Emptying, StaffWork.Teleporting, type, StaffWork.Emptying);
        }
        else if (gridIndex == 1)
        {
            //아이템이 있으면 창고로 가서 비우고, 없으면 바로 작업하러 가기
            if (IsInventoryEmpty())
            {
                if (type == StaffWork.Carrying)
                {
                    WorkStateMachine(StaffWork.Carrying, StaffWork.Teleporting, type, StaffWork.Checking);
                    checkingItems.Clear();
                }
                else if(type == StaffWork.Deliverying)
                {
                    WorkStateMachine(StaffWork.Deliverying, StaffWork.Checking, type, type);
                    checkingItems.Clear();
                }
                else
                {
                    WorkStateMachine(type, type, type, type);
                }
            }
            else
            {   //인벤에 아이템이 있다면 비우러 가기
                WorkStateMachine(type, StaffWork.Emptying, type, nextWorkType);
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
        if (command == StaffWork.Emptying)
        {
            yield return StartCoroutine(EmptyingRoutine());
        }
        //마켓 운반 명령, 유닛이 생성되면 시작하는 기본명령
        else if (command == StaffWork.Carrying)
        {
            yield return StartCoroutine(CarryingRoutine());
        }
        //공장 운반 명령
        else if( command == StaffWork.Deliverying)
        {
            yield return StartCoroutine(DeliveryingRoutine());
        }
        //차원포탈 작업루틴 : 벌목,채광,채집,사냥,낚시
        else if (IsDimensionWork(command))
        {
            yield return StartCoroutine(DimentionWorkRoutine());
        }
        //공장 작업루틴 : 쿠킹,컷팅,드라잉,쥬싱,멜팅,믹싱,패키징
        else if(IsFactoryWork(command))
        {
            yield return StartCoroutine(FactoryWorkRoutine());
        }

        //TODO:: 작업에 따라 추가
    }

    IEnumerator EmptyingRoutine()
    {
        if (workType == StaffWork.Teleporting)
        {
            yield return null;
            Teleport();
            if (IsInventoryEmpty())
            {
                if (receivedCommand == StaffWork.Carrying)
                {
                    WorkStateMachine(StaffWork.Carrying, StaffWork.Teleporting, receivedCommand, StaffWork.Checking);
                    checkingItems.Clear();
                }
                else if(receivedCommand == StaffWork.Deliverying)
                {
                    WorkStateMachine(StaffWork.Deliverying, StaffWork.Checking, receivedCommand, receivedCommand);
                    checkingItems.Clear();
                }
                else
                {
                    WorkStateMachine(receivedCommand, receivedCommand, receivedCommand, receivedCommand);
                }

            }
            else
            {   //인벤에 아이템이 있다면 비우러 가기
                WorkStateMachine(receivedCommand, StaffWork.Emptying, receivedCommand, nextWorkType);
            }
        }
    }

    IEnumerator CarryingRoutine()
    {
        //아이템 재고 확인중
        if (workType == StaffWork.Checking)
        {
            yield return RunningCoroutine(Checking());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 창고로 가기(인벤토리 가득참)
            {
                WorkStateMachine(command, StaffWork.Teleporting, command, StaffWork.Finding);
            }
            else
            {
                GoMarket();
            }

        }
        //창고에서 아이템 찾기
        else if (workType == StaffWork.Finding)
        {
            yield return RunningCoroutine(Finding());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 가기(인벤토리 가득참)
            {
                WorkStateMachine(command, StaffWork.Teleporting, command, StaffWork.Carrying);
            }
            else
            {
                GoWarehouse(checkingItems[workCount].checkedItem);
            }
        }
        //창고에서 판매대로 아이템 옮기기(아이템 진열)
        else if (workType == StaffWork.Carrying)
        {
            yield return RunningCoroutine(Carrying());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 다시 창고로 가기
            {
                if (IsInventoryEmpty()) //만약 남은 아이템이 없고, 다시 판매대 확인하러 가기
                {
                    WorkStateMachine(command, StaffWork.Checking, command, StaffWork.Checking);
                    checkingItems.Clear();
                }
                else                    //만약 남은 아이템이 있다면, 창고로 가서 아이템 비우기
                {
                    WorkStateMachine(command, StaffWork.Teleporting, command, StaffWork.Emptying);
                }
            }
            else
            {
                target = checkingItems[workCount].shelf.GetFrontPosition(checkingItems[workCount].frontIndex); //확인리스트의 판매대로 타겟지정
            }

        }
        //남은 아이템 창고로 옮기기
        else if (workType == StaffWork.Emptying)
        {
            yield return RunningCoroutine(Emptying());
            if (workCount >= invenSizeAvailable)   //작업 횟수가 사용가능한 인벤 개수와 같으면 판매대로 확인하러 가기(인벤토리 비움)
            {
                WorkStateMachine(command, StaffWork.Teleporting, command, StaffWork.Checking);
                checkingItems.Clear();
            }
            else
            {
                //yield return StartCoroutine("Emptying"); 여기에 있으면 아래 workCount에서 인덱스 에러 발생함
                GoWarehouse(inventory[workCount]); //확인리스트의 판매대로 타겟지정
            }

        }
        //포탈 텔레포트
        else if (workType == StaffWork.Teleporting)
        {
            yield return null;
            Teleport();
            WorkStateMachine(command, nextWorkType, command, nextWorkType);
        }
    }

    IEnumerator DeliveryingRoutine()
    {
        if(workType == StaffWork.Checking)
        {
            yield return RunningCoroutine(Checking(true));
            if(workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Finding, command, StaffWork.Finding);
            }
            else
            {
                target = factory.GetRandomFrontPosition(target);
            }
        }
        else if(workType == StaffWork.Finding)
        {
            yield return RunningCoroutine(Finding());
            if(workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Deliverying, command, StaffWork.Deliverying);
            }
            else
            {
                GoWarehouse(checkingItems[workCount].checkedItem);
            }
        }
        else if(workType == StaffWork.Deliverying)
        {
            yield return RunningCoroutine(Deliverying());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Loading, command, StaffWork.Loading);
            }
            else
            {
                target = checkingItems[workCount].factory.GetRandomFrontPosition(target);
            }
        }
        else if(workType == StaffWork.Loading) 
        {
            yield return RunningCoroutine(Loading());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Emptying, command, StaffWork.Emptying);
            }
            else
            {
                target = factory.GetRandomFrontPosition(target);
            }
        }
        else if(workType == StaffWork.Emptying)
        {
            yield return RunningCoroutine(Emptying());
            if(workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Checking, command, StaffWork.Checking);
                checkingItems.Clear();
            }
            else
            {
                GoWarehouse(inventory[workCount]);
            }
        }
    }

    IEnumerator DimentionWorkRoutine()
    {
        if (workType == command)
        {
            //TODO:: 하이드 - 디멘션입장 - 자원채취 - 창고로 옮기기
            yield return RunningCoroutine(DimensionWork());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, StaffWork.Emptying, command, workType);
                workCount = 0;
            }
            else
            {
                GoDimensionPortal(target);
            }
        }
        else if (workType == StaffWork.Emptying)
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

    IEnumerator FactoryWorkRoutine()
    {
        if (workType == command)
        {
            yield return RunningCoroutine(FactoryWork());
        }
        else if (workType == StaffWork.Emptying)
        {
            yield return RunningCoroutine(Emptying());
            if (workCount >= invenSizeAvailable)
            {
                WorkStateMachine(command, command, command, command);
            }
            else
            {
                GoWarehouse(inventory[workCount]);
            }
        }
    }

    IEnumerator Checking(bool isFactory = false)  //운반할 아이템 확인
    {
        waiting = WaitingCoroutine(Waiting(checkingTime));
        yield return waiting;
        if (isFactory)
        {
            factoryIndex = factory.GetRandomFactoryIndex();
            Item material = factory.GetRecipe(factoryIndex).items[workCount];
            checkingItems.Add(new CheckingItem(factory, material));
        }
        else
        {
            int shelfIndex = shelf.FindIndexOfFrontPosition(target);      //옮길 아이템 인덱스
            if (shelfIndex == -1) shelfIndex = Random.Range(0, shelf.maxInvenSize);            //만약 -1이면 아무 인덱스 넣기,확인중에 건물을 옮기면 -1이 나옴
            Item shelfItem = shelf.GetItemInInven(shelfIndex);           //옮길 아이템
            while (shelfItem == null)                                  //아이템이 없으면 다른 매대 찾기
            {
                shelfItem = BuildingManager.instance.GetItemInRandomWarehouse();   //아이템이 없다면 새아이템으로 채워넣기
            }

            checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem));       //확인리스트에 아이템 저장
        }
        ++workCount;
    }

    IEnumerator Finding() //아이템 찾기(창고에서 내 인벤토리로 아이템 옮기기)
    {
        yield return WaitingCoroutine(Waiting(checkingTime));
        int itemIndex = warehouse.FindItemIndexInInventory(checkingItems[workCount].checkedItem);
        if (itemIndex > -1)                                             //찾을 아이템이 존재한다면,1중
        {
            Item itemFound = warehouse.GetItemInInven(itemIndex);            //찾은 아이템
            int amount = Mathf.Clamp(itemFound.amount, 0, stat.GetWorkingAmount(command));
            if (PutItemInInventory(itemFound, amount) && itemFound.amount <= 0)
                warehouse.EmptyInventory(itemIndex);
        }
        ++workCount;
    }

    IEnumerator Carrying() //아이템 진열(내 인벤토리에서 판매대로 아이템 옮기기)
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
                Debug.Log("창고에 " + checkingItems[workCount].checkedItem.name + "가 없네");
            }
        }
        ++workCount;
    }

    IEnumerator Deliverying() //아이템 공장으로 배달
    {
        yield return WaitingCoroutine(Waiting(stat.GetWorkingTime(command)));
        if (inventory[workCount] != null)
        {
            if (factory.FindItemIndexInInventory(inventory[workCount]) > -1)
            {
                int index = factory.FindItemIndexInInventory(inventory[workCount]);
                EjectItemInInventory(factory.GetItemInInventory(index), inventory[workCount].amount);
            }
            else
            {
                Item newItem = new Item(inventory[workCount]);
                EjectItemInInventory(newItem, inventory[workCount].amount);
                factory.AddItemInInventoty(newItem);
            }
        }
        ++workCount;
    }

    IEnumerator Loading() //공장에서 필요없는 아이템 가져오기
    {
        yield return WaitingCoroutine(Waiting(stat.GetWorkingTime(command)));
        if (inventory[workCount] == null)
        {
            Item notNeed = factory.UnnecessaryItem();
            if(notNeed != null)
            {
                if (PutItemInInventory(notNeed, Mathf.Clamp(notNeed.amount, 0, stat.GetWorkingAmount(command))) && notNeed.amount <= 0)
                    factory.RemoveItemInInventory(notNeed);
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
        Item product = DimensionManager.instance.GetItem(dimension, command);   //아이템 가져오기
        if (product != null && product.amount != 0)
        {
            int amount = Mathf.Clamp(product.amount, 1, stat.GetWorkingAmount(command));
            PutItemInInventory(product, amount);
        }
        ++workCount;
    }

    IEnumerator FactoryWork() //쿠킹,컷팅,드라잉...
    {
        if(factory.IsEmptyStaff())
        {
            factory.EnterFactory(this, out factory, out factoryIndex);
            //TODO:: 하이드
            while (factory.FindIndexOfFrontPosition((Vector2)transform.position) > -1)
            {
                if(factory.IsReadyMaterial(factoryIndex))
                {
                    factory.ProductionInProgress(factoryIndex, true);
                    yield return WaitingCoroutine(Waiting(stat.GetWorkingTime(command)));
                    factory.CreateProduct(factoryIndex);
                    factory.ProductionInProgress(factoryIndex, false);
                }
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            //TODO::다음 팩토리 찾기
        }
        yield break;
    }

    public void ShiftDimension(Dimension _dimension)
    {
        if (dimension == _dimension) return;
        DimensionManager.instance.ShiftDimension(dimension, _dimension, this);
        //차원이동시 차원작업만 수행햐야됨, 기본으로 채광선택
        if(!IsDimensionWork(command) && _dimension != Dimension.Astaria)  
            ReceiveCommand(StaffWork.Mining);
        dimension = _dimension;
    }

    public bool IsDimensionWork(StaffWork _command) //차원포탈을 통한 작업 : 벌목,채광,채집,사냥,낚시
    {
        return _command switch
        {
            StaffWork.Felling or StaffWork.Mining or StaffWork.Collecting or StaffWork.Hunting or StaffWork.Fishing => true,
            _ => false
        };

    }

    public bool IsFactoryWork(StaffWork _command) //공장작업 : 쿠킹,컷팅,드라잉,쥬싱,멜팅,믹싱,패키징
    {
        return _command switch
        {
            StaffWork.Cooking or StaffWork.Cutting or StaffWork.Drying or StaffWork.Juicing or StaffWork.Melting or StaffWork.Mixing or StaffWork.Packaging => true,
            _ => false
        };

    }

    bool PutItemInInventory(Item itemFound, int amount) //아이템을 내 인벤에 넣기, 창고이면 인벤 인덱스 값을 넣고 창고가 아니면 index에 -1
    {
        inventory[workCount] = new Item(itemFound);
        if (itemFound.MinusAmount(amount))
        {
            inventory[workCount].PlusAmount(amount);
            return true;
        }

        return false;
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
        warehouse = BuildingManager.instance.FindItemInWarehouseList(itemToFind);
        if(warehouse == null)
        {
            warehouse = BuildingManager.instance.FindEmptyWarehouse(); //아이템이 없다면 빈창고로 가기
            if (warehouse == null)
                warehouse = BuildingManager.instance.RequestRandomWarehouse(); //아이템이 없고, 빈창고도 없으면 그냥 아무창고에 가기
        }

        target = warehouse.GetRandomFrontPosition(target);
    }

    void GoPortal()
    {
        if (gridIndex == 0)
            target = GameManager.instance.portals[0].GetFrontPosition();
        else if (gridIndex == 1)
            target = GameManager.instance.portals[1].GetFrontPosition();
    }

    void GoDimensionPortal(Vector2 _target)
    {
        do
        {
            target = GameManager.instance.portals[2].GetFrontPosition();
        } while (target == _target);
        
    }

    void GoFactory(StaffWork workType)
    {
        if (!IsFactoryWork(workType))
            factory = BuildingManager.instance.RequestRandomFactory();
        else
            factory = BuildingManager.instance.RequestEmptyFactory(workType);
        
        target = factory.GetFrontPosition();
    }

    //재고 확인 구조체
    public class CheckingItem
    {
        public Shelf shelf;
        public Factory factory;
        public int frontIndex;
        public Item checkedItem;

        public CheckingItem(Shelf _shelf, int _frontIndex, Item _shelfItem)
        {
            shelf = _shelf;
            frontIndex = _frontIndex;
            checkedItem = _shelfItem;
        }

        public CheckingItem(Factory _factory, Item material)
        {
            factory = _factory;
            checkedItem = material;
        }
    }
}



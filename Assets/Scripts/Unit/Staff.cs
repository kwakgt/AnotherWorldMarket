using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Staff : Unit
{
    int amountOfCarrying = 20;  //�ѹ��� ��ݰ����� �ִ밳��
    int workTime = 1;

    Warehouse warehouse;
    WorkType workType;          //���� �÷���
    List<CheckingItem> checkingItems = new List<CheckingItem>();    //Ȯ��ǰ�񸮽�Ʈ
    
    int workCount;                  //�۾��� ����Ƚ��,�۾��ε���,�κ��ε���
    float checkingTime = 0.5f;      //Ȯ�νð�
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

        if (workType == WorkType.Checking)          //�Ŵ뿡 ������ Ȯ��
        {
            yield return StartCoroutine("Checking");
            if (workCount >= invenSizeAvailable || checkingItems.Count >= invenSizeAvailable)   //�۾� Ƚ���� ��밡���� �κ� ������ ������ â���� ����(�κ��丮 ������)
            {
                workType = WorkType.Finding;
                workCount = 0;
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
            else
            {
                GoMarket();
            }

        }
        else if (workType == WorkType.Finding)       //â������ ������ ã��
        {
            yield return StartCoroutine("Finding");
            if (workCount >= invenSizeAvailable)   //�۾� Ƚ���� ��밡���� �κ� ������ ������ �ǸŴ�� ����(�κ��丮 ������)
            {
                workType = WorkType.Carrying;
                workCount = 0;
                target = checkingItems[workCount].shelf.GetShelfFrontPosition(checkingItems[workCount].frontIndex); //ù��° Ȯ�θ���Ʈ�� �ǸŴ�� Ÿ������
            }
            else
            {
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
        }
        else if (workType == WorkType.Carrying)     //ã�� ���ǵ� �Ŵ�� ���� �ű��
        {
            yield return StartCoroutine("Carrying");
            if (workCount >= invenSizeAvailable)   //�۾� Ƚ���� ��밡���� �κ� ������ ������ �ٽ� â���� ����(�κ��丮 ���)
            {
                workType = WorkType.Emptying;
                workCount = 0;
                GoWarehouse(checkingItems[workCount].shlefItem);
            }
            else
            {
                target = checkingItems[workCount].shelf.GetShelfFrontPosition(checkingItems[workCount].frontIndex); //Ȯ�θ���Ʈ�� �ǸŴ�� Ÿ������
            }

        }
        else if (workType == WorkType.Emptying)     //���� �������� ������ â���� �ٽ� �ֱ�
        {
            if(IsInventoryEmpty())  //���� ���� �������� ����, �κ��丮�� ����ٸ� �ٽ� �ǸŴ� Ȯ���Ϸ� ����
            {
                workType = WorkType.Checking;
                checkingItems.Clear();
                workCount = 0;
                GoMarket();
            }
            else
            {
                yield return StartCoroutine("Emptying");
                if (workCount >= invenSizeAvailable)   //�۾� Ƚ���� ��밡���� �κ� ������ ������ �ǸŴ�� Ȯ���Ϸ� ����(�κ��丮 ���)
                {
                    workType = WorkType.Checking;
                    checkingItems.Clear();
                    workCount = 0;
                    GoMarket();
                }
                else
                {
                    //yield return StartCoroutine("Emptying"); ���⿡ ������ �Ʒ� workCount���� �ε��� ���� �߻���
                    target = warehouse.GetWarehouseFrontPosition(target); //Ȯ�θ���Ʈ�� �ǸŴ�� Ÿ������
                }
            }
        }
 
        //TODO:: �۾��� ���� �߰�
    }


    IEnumerator Checking()
    {
        yield return StartCoroutine("Waiting", checkingTime);
        Item shelfItem = shelf.FindItemInSlot(target);                                                      //�ű� ������
        int shelfIndex = shelf.FindItemSlotIndex(target);                                                   //������ �ε���
        int amountCarring = Mathf.Clamp(shelfItem.amountOfShelf - shelfItem.amount, 1, amountOfCarrying);   //�ű� ����
        if (shelfItem == null)                                                                              //�������� ������ �ٸ� �Ŵ� ã��
        {
            GoMarket();
            yield break;
        }
        
        checkingItems.Add(new CheckingItem(shelf, shelfIndex, shelfItem, amountCarring));       //Ȯ�θ���Ʈ�� ������ ����
        Debug.Log(workCount + " , " + shelfItem.name + " , " + amountCarring);
        ++workCount;
    }

    IEnumerator Finding()
    {
        yield return StartCoroutine("Waiting", workTime);
        Item itemToFind = checkingItems[workCount].shlefItem;           //ã�� ������
        int itemIndex = warehouse.FindItemIndexInInventory(itemToFind); //ã�� �������ε���
        if (itemIndex < 0) yield break;
        Item itemFound = warehouse.inventory[itemIndex];                //ã�� ������

        if (itemFound != null)
        {
            Debug.Log("������ �ֱ�");
            int maxAmountCarring = Mathf.Min(amountOfCarrying, itemFound.amount);
            int amount =Mathf.Clamp(checkingItems[workCount].amountCarring, 1, maxAmountCarring);
            if (warehouse.FindItemInWarehouse(itemFound))               //ã�� �������� â���� �ִٸ�
            {
                PutItemInInventory(itemFound, itemIndex, amount); //�κ��丮�� �ű��
            }
        }
        ++workCount;
    }

    IEnumerator Carrying()
    {
        yield return StartCoroutine("Waiting", workTime);
        Item shelfItem = checkingItems[workCount].shelf.ItemSlot[checkingItems[workCount].frontIndex];  //�ǸŴ� ������
        if (shelfItem.Equals(inventory[workCount]))   //�ǸŴ� �����۰� �� �κ��丮 �������� ������
        {
            Debug.Log("������ ������");
            int maxAmountCarring = Mathf.Min(shelfItem.amountOfShelf - shelfItem.amount, amountOfCarrying);   //�ǸŴ뿡 ������ �ִ� ��� �� ��ݷ��߿� ���� ���� ����� MAX��
            int amount = Mathf.Clamp(inventory[workCount].amount, 1, maxAmountCarring);
            EjectItemInInventory(shelfItem, amount);
        }
        ++workCount;
    }

    IEnumerator Emptying()
    {
        yield return StartCoroutine("Waiting", workTime);
        if (inventory[workCount] != null)
        {
            int itemIndex = warehouse.FindItemIndexInInventory(inventory[workCount]);
            if(itemIndex != -1) //â���� ���� �������� �����ϸ�
            {
                EjectItemInInventory(warehouse.inventory[itemIndex], inventory[workCount].amount);   //â���� ������ �ֱ�
            }
            else //â���� ���� �������� ���ٸ�
            {
                //TODO:: ��ĭ�� �ֱ�
            }
        }

        ++workCount;
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
        Debug.Log("������ ������");
        shelfItem.PlusAmount(amount);
        inventory[workCount].MinusAmount(amount);
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
            target = warehouse.GetWarehouseFrontPosition(target);
            return;
        }
        else
            target = WarehouseManager.instance.RequestRandomWarehouse().GetWarehouseFrontPosition(target);
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


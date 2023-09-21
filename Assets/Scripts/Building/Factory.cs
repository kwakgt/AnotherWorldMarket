using EnumManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Factory : Structure
{
    public SpriteRenderer image;
    //�۾� Ÿ��
    public StaffWork workType { get; private set; }
    //���ϰ� �ִ� ����
    public Staff[] staffs { get; private set; } = new Staff[3];
    //������ ����Ʈ
    public List<Recipe> recipes { get; private set; } = new List<Recipe>();
    //���õ� ������
    public Recipe[] selectedRecipe { get; private set; } = new Recipe[3];
    //������ ���
    public List<Item[]> materials { get; private set; } = new List<Item[]>();
    //���ĭ ������
    int maxMaterialSize = 5;
    //�κ��丮
    public List<Item> inventory = new List<Item>();
    //�ִ� �κ��丮 ��
    int maxInventorySize = 30;
    //�������
    FactoryStatus[] status = new FactoryStatus[3] { FactoryStatus.Empty, FactoryStatus.Empty, FactoryStatus.Empty };

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(MovingMaterials());
    }

    IEnumerator MovingMaterials() //������ �������̸� �κ� -> ���ĭ, �ƴϸ� ���ĭ -> �κ�
    {
        int check = 0;  //���ĭ �ε���
        while(true)
        {
            for (int factoryIndex = 0; factoryIndex < staffs.Length; factoryIndex++)    //factoryIndex : ���� ���� �ε���
            {
                //������ ���ų� ������ �������� ������ �ǳʶٱ�
                if (staffs[factoryIndex] == null || selectedRecipe[factoryIndex].items[check] == null) continue;
                //�κ��丮���� ������ ã��
                int index = inventory.FindIndex(it => it.Equals(selectedRecipe[factoryIndex].items[check]));
                //if(index < 0) continue; :: ������ ���� �� else�� ���� �ȵ�
                if (materials[factoryIndex][check] == null)
                {
                    if (index < 0) continue;
                    //���ĭ�� �������� ������ â������ ������ ã�� �� ������ �ֱ�
                    int amount = Mathf.Clamp(inventory[index].amount, 0, staffs[factoryIndex].stat.GetWorkingAmount(workType));
                    materials[factoryIndex][check] = new Item(inventory[index]);
                    if (ExchangeItem(inventory[index], materials[factoryIndex][check], amount) && inventory[index].amount <= 0)
                        inventory.RemoveAt(index);

                }
                else if (materials[factoryIndex][check].Equals(selectedRecipe[factoryIndex].items[check]))
                {
                    if (index < 0) continue;
                    //������� maxMaterialAmount������ ������ �ǳʶٱ�
                    if (materials[factoryIndex][check].amount > materials[factoryIndex][check].amountOfFactory) continue;
                    int max = Mathf.Min(staffs[factoryIndex].stat.GetWorkingAmount(workType), materials[factoryIndex][check].amountOfFactory - materials[factoryIndex][check].amount);
                    int amount = Mathf.Clamp(inventory[index].amount, 0, max);
                    //���ĭ�� �������� �ְ� �����ǿ� ������ â������ ������ ã�� ������ �÷���
                    if (ExchangeItem(inventory[index], materials[factoryIndex][check], amount) && inventory[index].amount <= 0)
                        inventory.RemoveAt(index);
                }
                else
                {
                    //���ĭ�� �������� �ְ� �����ǿ� �ٸ��� ���ĭ���� ������ â���� �ű��
                    index = inventory.FindIndex(it => it.Equals(materials[factoryIndex][check]));
                    if (index < 0)
                    {
                        //â���� �������� ������ â���� ������ �߰� �� ���ĭ ������ ����
                        inventory.Add(new Item(materials[factoryIndex][check], materials[factoryIndex][check].amount));
                        materials[factoryIndex][check] = null;
                    }
                    else
                    {
                        //â���� ������ ������ ���� ��ġ��, ���ĭ ������ ����
                        int amount = Mathf.Clamp(materials[factoryIndex][check].amount, 0, inventory[index].amountOfFactory - inventory[index].amount);
                        if (ExchangeItem(materials[factoryIndex][check], inventory[index], materials[factoryIndex][check].amount) && materials[factoryIndex][check].amount <= 0)
                            materials[factoryIndex][check] = null;
                    }
                }
                yield return new WaitForSeconds(0.25f);
            }

            if (++check == maxMaterialSize)
                check = 0;
            yield return new WaitForSeconds(1f);
        }
    }

    bool ExchangeItem(Item from, Item to, int amount) //������ ���� + -
    {
        if (from.MinusAmount(amount))
        {
            to.PlusAmount(amount);
            return true;
        }
        else
            return false;
    }

    public void SetFactory(Sprite sprite, StaffWork _workType) //���� �ʱ�ȭ
    {
        image.sprite = sprite;
        workType = _workType;
        recipes = ItemManager.instance.GetRecipe(workType);
        uniIndex = BuildingManager.instance.RequestFactoryIndex();
        BuildingManager.instance.AddFactoryDictionary(uniIndex, workType, this);
        InitMaterials();
        SelectRecipe(0, 0);
        SelectRecipe(1, 0);
        SelectRecipe(2, 0);
    }

    void InitMaterials() //���ĭ �ʱ�ȭ
    {
        for (int i = 0; i < staffs.Length; i++)
            materials.Add(new Item[maxMaterialSize]);
    }

    public bool IsReadyMaterial(int factoryIndex) //��� �غ� ���� ����
    {
        return selectedRecipe[factoryIndex].CompareMaterial(materials[factoryIndex]);
    }

    public void CreateProduct(int factoryIndex) //��ǰ ����
    {
        if (selectedRecipe[factoryIndex].product == null) return;

        //��� ���� �� �ϼ�ǰ ����
        for (int i = 0; i < selectedRecipe[factoryIndex].items.Length; i++)
        {
            Item item = selectedRecipe[factoryIndex].items[i];
            if (item == null) continue;

            if (item.Equals(materials[factoryIndex][i]))
            {
                materials[factoryIndex][i].MinusAmount(item.amount);
                if (materials[factoryIndex][i].amount <= 0)
                {
                    materials[factoryIndex][i] = null;
                }
            }
            else
            {
                return; //��ᰡ �ٸ��� ���� ���
            }

        }
        int idx = inventory.FindIndex(it => it.Equals(selectedRecipe[factoryIndex].product));
        if (idx > -1)
            inventory[idx].PlusAmount(selectedRecipe[factoryIndex].product.amount);
        else
            inventory.Add(new Item(selectedRecipe[factoryIndex].product, selectedRecipe[factoryIndex].product.amount));
    }

    public void EnterFactory(Staff staff, out Factory factory, out int factoryIndex) //���� ��
    {
        factory = null;
        factoryIndex = -1;
        for(int i = 0; i < staffs.Length; i++)
        {
            if (staffs[i] == null)
            {
                staffs[i] = staff;
                factory = this;
                factoryIndex = i;
                status[i] = FactoryStatus.Wait;
                break;
            }
        }
    }

    public void ExitFactory(int factoryIndex) //���� ����
    {
        staffs[factoryIndex] = null;
        status[factoryIndex] = FactoryStatus.Empty;
        EmptyingMaterials(factoryIndex);
    }

    void EmptyingMaterials(int factoryIndex) //������ ������ ���ĭ�� ������ �κ����� �ű��
    {
        for (int i = 0; i < maxMaterialSize; i++)
        {
            if (materials[factoryIndex][i] == null) continue;

            int index = inventory.FindIndex(it => it.Equals(materials[factoryIndex][i]));
            if (index < 0)
            {
                inventory.Add(new Item(materials[factoryIndex][i], materials[factoryIndex][i].amount));
                materials[factoryIndex][i] = null;
            }
            else
            {
                if (ExchangeItem(materials[factoryIndex][i], inventory[index], materials[factoryIndex][i].amount) && materials[factoryIndex][i].amount <= 0)
                    materials[factoryIndex][i] = null;
            }
        }
    }

    public void ProductionInProgress(int factoryIndex, bool inProgress) //���� ���� ����
    {
        if (inProgress) status[factoryIndex] = FactoryStatus.Produce;
        else status[factoryIndex] = FactoryStatus.Wait;
    }

    public bool IsEmptyStaff() //���� ���ڸ� ����
    {
        for(int i = 0 ; i < status.Length; i++)
        {
            if (status[i] == FactoryStatus.Empty)
                return true;
        }
        return false;
    }

    public int GetRandomFactoryIndex() //���� �ִ� �ڸ� �� ���� �ڸ�, Deliverying�� �� ���
    {
        List<int> list = new List<int>();
        for (int i = 0; i < staffs.Length; i++)
        {
            if (staffs[i] != null)
                list.Add(i);
        }

        return (list.Count == 0) ? Random.Range(0, list.Count) : list[Random.Range(0, list.Count)];
    }

    public void SelectRecipe(int factoryIndex, int recipeIndex) //���� ������ ����
    {
        selectedRecipe[factoryIndex] = recipes[recipeIndex];
    }

    public Recipe GetRecipe(int factoryIndex) //���õ� ������
    {
        return selectedRecipe[factoryIndex];
    }

    public Item GetItemInInventory(int index) //������ ���
    {
        return inventory[index];
    }

    public int FindItemIndexInInventory(Item found, bool isLast = false) //������ ã��
    {
        if(isLast)
            return inventory.FindLastIndex(it => it.Equals(found));
        else
            return inventory.FindIndex(it => it.Equals(found));
    }

    public void AddItemInInventoty(Item newItem) //�κ��� ������ �߰�
    {
        if(inventory.Count < maxInventorySize)
            inventory.Add(newItem);
    }

    public void RemoveItemInInventory(Item item) //�κ� ������ ����
    {
        inventory.Remove(item);
    }

    public Item UnnecessaryItem(bool isLast = false)    //�ʿ���� ������ ���
    {
        if (isLast) //�ڿ������� Ȯ��
        {
            for (int i = inventory.Count - 1; i > -1; i--)
            {
                if (inventory[i] == null) continue;
                bool isMaterial = false;
                for (int j = 0; j < selectedRecipe.Length; j++) //�������� 3���� �����ǿ� ��
                {
                    if (staffs[j] == null) continue;    //������ ������ �ǳʶٱ�
                    isMaterial |= selectedRecipe[j].ContainToRecipe(inventory[i]); //3���� ������ �� �ϳ��� ���Եȴٸ� true
                }

                if (!isMaterial)    //��ᰡ �ƴϸ� �ʿ���� �������̹Ƿ� â���� �ű�
                    return inventory[i];
            }
        }
        else //�տ������� Ȯ��
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null) continue;
                bool isMaterial = false;
                for (int j = 0; j < selectedRecipe.Length; j++) //�������� 3���� �����ǿ� ��
                {
                    if (staffs[j] == null) continue;    //������ ������ �ǳʶٱ�
                    isMaterial |= selectedRecipe[j].ContainToRecipe(inventory[i]); //3���� ������ �� �ϳ��� ���Եȴٸ� true
                }

                if (!isMaterial)    //��ᰡ �ƴϸ� �ʿ���� �������̹Ƿ� â���� �ű�
                    return inventory[i];
            }
        }
        return null;
    }

    public int MaxMaterialSize { get { return maxMaterialSize; } }

    public int InventorySize { get { return inventory.Count; } }
}
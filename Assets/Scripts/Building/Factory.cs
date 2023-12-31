using EnumManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Factory : Structure
{
    public SpriteRenderer image;
    //작업 타입
    public StaffWork workType { get; private set; }
    //일하고 있는 직원
    public Staff[] staffs { get; private set; } = new Staff[3];
    //레시피 리스트
    public List<Recipe> recipes { get; private set; } = new List<Recipe>();
    //선택된 레시피
    public Recipe[] selectedRecipe { get; private set; } = new Recipe[3];
    //레시피 재료
    public List<Item[]> materials { get; private set; } = new List<Item[]>();
    //재료칸 사이즈
    int maxMaterialSize = 5;
    //인벤토리
    public List<Item> inventory = new List<Item>();
    //최대 인벤토리 수
    int maxInventorySize = 30;
    //공장상태
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

    IEnumerator MovingMaterials() //레시피 아이템이면 인벤 -> 재료칸, 아니면 재료칸 -> 인벤
    {
        int check = 0;  //재료칸 인덱스
        while(true)
        {
            for (int factoryIndex = 0; factoryIndex < staffs.Length; factoryIndex++)    //factoryIndex : 공장 슬롯 인덱스
            {
                //직원이 없거나 레시피 아이템이 없으면 건너뛰기
                if (staffs[factoryIndex] == null || selectedRecipe[factoryIndex].items[check] == null) continue;
                //인벤토리에서 아이템 찾기
                int index = inventory.FindIndex(it => it.Equals(selectedRecipe[factoryIndex].items[check]));
                //if(index < 0) continue; :: 레시피 변경 시 else문 수행 안됨
                if (materials[factoryIndex][check] == null)
                {
                    if (index < 0) continue;
                    //재료칸에 아이템이 없으면 창고에서 아이템 찾아 새 아이템 넣기
                    int amount = Mathf.Clamp(inventory[index].amount, 0, staffs[factoryIndex].stat.GetWorkingAmount(workType));
                    materials[factoryIndex][check] = new Item(inventory[index]);
                    if (ExchangeItem(inventory[index], materials[factoryIndex][check], amount) && inventory[index].amount <= 0)
                        inventory.RemoveAt(index);

                }
                else if (materials[factoryIndex][check].Equals(selectedRecipe[factoryIndex].items[check]))
                {
                    if (index < 0) continue;
                    //재료템이 maxMaterialAmount개보다 많으면 건너뛰기
                    if (materials[factoryIndex][check].amount > materials[factoryIndex][check].amountOfFactory) continue;
                    int max = Mathf.Min(staffs[factoryIndex].stat.GetWorkingAmount(workType), materials[factoryIndex][check].amountOfFactory - materials[factoryIndex][check].amount);
                    int amount = Mathf.Clamp(inventory[index].amount, 0, max);
                    //재료칸에 아이템이 있고 레시피와 같으면 창고에서 아이템 찾아 수량만 플러스
                    if (ExchangeItem(inventory[index], materials[factoryIndex][check], amount) && inventory[index].amount <= 0)
                        inventory.RemoveAt(index);
                }
                else
                {
                    //재료칸에 아이템이 있고 레시피와 다르면 재료칸에서 아이템 창고로 옮기기
                    index = inventory.FindIndex(it => it.Equals(materials[factoryIndex][check]));
                    if (index < 0)
                    {
                        //창고에 아이템이 없으면 창고에 아이템 추가 후 재료칸 아이템 삭제
                        inventory.Add(new Item(materials[factoryIndex][check], materials[factoryIndex][check].amount));
                        materials[factoryIndex][check] = null;
                    }
                    else
                    {
                        //창고에 아이템 있으면 수량 합치고, 재료칸 아이템 삭제
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

    bool ExchangeItem(Item from, Item to, int amount) //아이템 수량 + -
    {
        if (from.MinusAmount(amount))
        {
            to.PlusAmount(amount);
            return true;
        }
        else
            return false;
    }

    public void SetFactory(Sprite sprite, StaffWork _workType) //공장 초기화
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

    void InitMaterials() //재료칸 초기화
    {
        for (int i = 0; i < staffs.Length; i++)
            materials.Add(new Item[maxMaterialSize]);
    }

    public bool IsReadyMaterial(int factoryIndex) //재료 준비 상태 여부
    {
        return selectedRecipe[factoryIndex].CompareMaterial(materials[factoryIndex]);
    }

    public void CreateProduct(int factoryIndex) //제품 생성
    {
        if (selectedRecipe[factoryIndex].product == null) return;

        //재료 삭제 후 완성품 제공
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
                return; //재료가 다르면 생산 취소
            }

        }
        int idx = inventory.FindIndex(it => it.Equals(selectedRecipe[factoryIndex].product));
        if (idx > -1)
            inventory[idx].PlusAmount(selectedRecipe[factoryIndex].product.amount);
        else
            inventory.Add(new Item(selectedRecipe[factoryIndex].product, selectedRecipe[factoryIndex].product.amount));
    }

    public void EnterFactory(Staff staff, out Factory factory, out int factoryIndex) //공장 들어감
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

    public void ExitFactory(int factoryIndex) //공장 나감
    {
        staffs[factoryIndex] = null;
        status[factoryIndex] = FactoryStatus.Empty;
        EmptyingMaterials(factoryIndex);
    }

    void EmptyingMaterials(int factoryIndex) //직원이 나가면 재료칸의 아이템 인벤으로 옮기기
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

    public void ProductionInProgress(int factoryIndex, bool inProgress) //공장 상태 변경
    {
        if (inProgress) status[factoryIndex] = FactoryStatus.Produce;
        else status[factoryIndex] = FactoryStatus.Wait;
    }

    public bool IsEmptyStaff() //직원 빈자리 여부
    {
        for(int i = 0 ; i < status.Length; i++)
        {
            if (status[i] == FactoryStatus.Empty)
                return true;
        }
        return false;
    }

    public int GetRandomFactoryIndex() //직원 있는 자리 중 랜덤 자리, Deliverying일 때 사용
    {
        List<int> list = new List<int>();
        for (int i = 0; i < staffs.Length; i++)
        {
            if (staffs[i] != null)
                list.Add(i);
        }

        return (list.Count == 0) ? Random.Range(0, list.Count) : list[Random.Range(0, list.Count)];
    }

    public void SelectRecipe(int factoryIndex, int recipeIndex) //선택 레시피 변경
    {
        selectedRecipe[factoryIndex] = recipes[recipeIndex];
    }

    public Recipe GetRecipe(int factoryIndex) //선택된 레시피
    {
        return selectedRecipe[factoryIndex];
    }

    public Item GetItemInInventory(int index) //아이템 얻기
    {
        return inventory[index];
    }

    public int FindItemIndexInInventory(Item found, bool isLast = false) //아이템 찾기
    {
        if(isLast)
            return inventory.FindLastIndex(it => it.Equals(found));
        else
            return inventory.FindIndex(it => it.Equals(found));
    }

    public void AddItemInInventoty(Item newItem) //인벤에 아이템 추가
    {
        if(inventory.Count < maxInventorySize)
            inventory.Add(newItem);
    }

    public void RemoveItemInInventory(Item item) //인벤 아이템 제거
    {
        inventory.Remove(item);
    }

    public Item UnnecessaryItem(bool isLast = false)    //필요없는 아이템 얻기
    {
        if (isLast) //뒤에서부터 확인
        {
            for (int i = inventory.Count - 1; i > -1; i--)
            {
                if (inventory[i] == null) continue;
                bool isMaterial = false;
                for (int j = 0; j < selectedRecipe.Length; j++) //아이템을 3개의 레시피와 비교
                {
                    if (staffs[j] == null) continue;    //직원이 없으면 건너뛰기
                    isMaterial |= selectedRecipe[j].ContainToRecipe(inventory[i]); //3개의 레시피 중 하나라도 포함된다면 true
                }

                if (!isMaterial)    //재료가 아니면 필요없는 아이템이므로 창고로 옮김
                    return inventory[i];
            }
        }
        else //앞에서부터 확인
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null) continue;
                bool isMaterial = false;
                for (int j = 0; j < selectedRecipe.Length; j++) //아이템을 3개의 레시피와 비교
                {
                    if (staffs[j] == null) continue;    //직원이 없으면 건너뛰기
                    isMaterial |= selectedRecipe[j].ContainToRecipe(inventory[i]); //3개의 레시피 중 하나라도 포함된다면 true
                }

                if (!isMaterial)    //재료가 아니면 필요없는 아이템이므로 창고로 옮김
                    return inventory[i];
            }
        }
        return null;
    }

    public int MaxMaterialSize { get { return maxMaterialSize; } }

    public int InventorySize { get { return inventory.Count; } }
}

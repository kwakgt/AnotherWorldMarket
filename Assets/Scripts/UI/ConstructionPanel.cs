using EnumManager;
using System;
using UnityEngine;

public class ConstructionPanel : MonoBehaviour
{
    //프리팹
    public GameObject shelfPrefab;      //판매대 프리팹
    public GameObject warehousePrefab;  //창고 프리팹
    public GameObject factoryPrefab;    //공장 프리팹
    public GameObject shelfParent;      //판매대 부모
    public GameObject warehouseParent;  //창고 부모
    public GameObject factoryParent;    //공장 부모
    //자식 패널
    GameObject topPanel;
    GameObject buildingPanel;
    //건설
    Structure selected;         //설치할 구조체
    StructureName strucName;

    void Awake()
    {
        topPanel = transform.GetChild(0).gameObject;
        buildingPanel = transform.GetChild(1).gameObject;
    }

    void Start()
    {
        gameObject.SetActive(false);    //GameManager에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    void Update()
    {
        StartConstruction();
    }

    void StartConstruction()    //건설모드에서 지을 건물 클릭시 수행
    {
        if(GameManager.instance.Equals(GameMode.Building) && selected != null && strucName != StructureName.None)
        {
            selected.OnMoving();    //건물 선택 시 이동모드 진행

            if (!selected.IsMoving) //설치가 완료되면 변수 초기화
            {
                strucName = StructureName.None;
                selected = null;
            }
        }
        else
        {
            strucName = StructureName.None;
            selected = null;
        }
    }

    //탭 선택
    public void SelectTab(int struc)
    {
        GameObject market = buildingPanel.transform.GetChild(0).gameObject;
        GameObject building = buildingPanel.transform.GetChild(1).gameObject;
        GameObject common = buildingPanel.transform.GetChild(2).gameObject;

        switch (struc)
        {
            case 0:
                market.SetActive(true);
                building.SetActive(false);
                common.SetActive(false);
                break;
            case 1:
                market.SetActive(false);
                building.SetActive(true);
                common.SetActive(false);
                break;
            case 2:
                market.SetActive(false);
                building.SetActive(false);
                common.SetActive(true);
                break;
            default:
                market.SetActive(true);
                building.SetActive(false);
                common.SetActive(false);
                break;
        }
    }

    //건축모드 버튼
    public void SelectedShelf() //Shelf 버튼 클릭
    {
        strucName = StructureName.Shelf;
        selected = Instantiate(shelfPrefab, shelfParent.transform).GetComponent<Shelf>();
        selected.IsMoving = true;           //이동모드 시작
        selected.IsNewStructure = true;     //새건물
    }

    public void SelectedWarehouse() //Warehouse 버튼 클릭
    {
        strucName = StructureName.Warehouse;
        selected = Instantiate(warehousePrefab, shelfParent.transform).GetComponent<Warehouse>();
        selected.IsMoving = true;
        selected.IsNewStructure = true;
    }

    public void SelectedFacotory(string work) //Factory 버튼 클릭
    {
        StaffWork staffWork = Enum.Parse<StaffWork>(work);
        Debug.Log(staffWork);
        strucName = StructureName.Factory;
        selected = Instantiate(factoryPrefab, factoryParent.transform).GetComponent<Factory>();
        selected.IsMoving = true;
        selected.IsNewStructure = true;

        Factory factory = selected as Factory;
        factory.SetFactory(SpriteManager.instance.GetFactoryImage(staffWork), staffWork);


    }

    enum StructureName { None, Shelf, Warehouse, Factory }
}

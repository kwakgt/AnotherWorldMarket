using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPanel : MonoBehaviour
{
    public GameObject shelfPrefab;      //판매대 프리팹
    public GameObject warehousePrefab;  //창고 프리팹
    public GameObject shelfParent;      //판매대 부모
    public GameObject warehouseParent;  //창고 부모

    Structure selected;         //설치할 구조체
    StructureName strucName;


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
        if(GameManager.instance.CompareTo(GameManager.GameMode.Builder) && selected != null && strucName != StructureName.None)
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

    enum StructureName { None, Shelf, Warehouse}
}

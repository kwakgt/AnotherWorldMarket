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
        gameObject.SetActive(false); //GameManager에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }

    void Update()
    {
        StartConstruction();
    }

    void StartConstruction()
    {
        if(GameManager.instance.CompareTo(GameManager.GameMode.Builder) && selected != null && strucName != StructureName.None)
        {
            selected.OnMoving();

            if (!selected.IsMoving)
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

    //건축모드
    public void SelectedShelf() //Shelf 버튼 클릭
    {
        strucName = StructureName.Shelf;
        selected = Instantiate(shelfPrefab, shelfParent.transform).GetComponent<Shelf>();
        selected.IsMoving = true;
    }

    public void SelectedWarehouse() //Warehouse 버튼 클릭
    {
        strucName = StructureName.Warehouse;
        selected = Instantiate(warehousePrefab, shelfParent.transform).GetComponent<Warehouse>();
        selected.IsMoving = true;
    }

    enum StructureName { None, Shelf, Warehouse}
}

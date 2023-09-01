using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPanel : MonoBehaviour
{
    public GameObject shelfPrefab;      //�ǸŴ� ������
    public GameObject warehousePrefab;  //â�� ������
    public GameObject shelfParent;      //�ǸŴ� �θ�
    public GameObject warehouseParent;  //â�� �θ�

    Structure selected;         //��ġ�� ����ü
    StructureName strucName;


    void Start()
    {
        gameObject.SetActive(false);    //GameManager���� GameObject.Find�Լ� ����� ���� Start���� ��Ȱ��ȭ, ��Ȱ��ȭ�� ������Ʈ�� Find�Լ��� ���� �ȵ�.
    }

    void Update()
    {
        StartConstruction();
    }

    void StartConstruction()    //�Ǽ���忡�� ���� �ǹ� Ŭ���� ����
    {
        if(GameManager.instance.CompareTo(GameManager.GameMode.Builder) && selected != null && strucName != StructureName.None)
        {
            selected.OnMoving();    //�ǹ� ���� �� �̵���� ����

            if (!selected.IsMoving) //��ġ�� �Ϸ�Ǹ� ���� �ʱ�ȭ
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


    //������ ��ư
    public void SelectedShelf() //Shelf ��ư Ŭ��
    {
        strucName = StructureName.Shelf;
        selected = Instantiate(shelfPrefab, shelfParent.transform).GetComponent<Shelf>();
        selected.IsMoving = true;           //�̵���� ����
        selected.IsNewStructure = true;     //���ǹ�
    }

    public void SelectedWarehouse() //Warehouse ��ư Ŭ��
    {
        strucName = StructureName.Warehouse;
        selected = Instantiate(warehousePrefab, shelfParent.transform).GetComponent<Warehouse>();
        selected.IsMoving = true;
        selected.IsNewStructure = true;
    }

    enum StructureName { None, Shelf, Warehouse}
}
using EnumManager;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
    }

    DataList dataList = new DataList();

    string path; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + "/Save";
    }

    public void SaveData()
    {
        //bool prettyPrint : �⺻ false, false ���ٷ� �����ؼ� ����, true �����ٿ� "Key : Value"�� �̻ڰ� ����
        string data = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(path, data);
        ListClear();
    }

    void ListClear()
    {
        dataList.unitDataList.Clear();
        dataList.structureDataList.Clear();
    }

    public void UpdateData(GameObject _gameObject)
    {
        if(_gameObject.GetComponent<Unit>())
        {
            UnitData uniData = new UnitData();
            uniData.position = _gameObject.transform.position;
            uniData.unitType = _gameObject.GetComponent<Unit>().type;
            //TODO:: �Ӽ� �߰�


            dataList.unitDataList.Add(uniData);
        }
        else if( _gameObject.GetComponent<Structure>())
        {
            //Structure struc = _gameObject.GetComponent<Structure>();
            StructureData strucData = new StructureData();
            strucData.position = _gameObject.transform.position;
            //TODO:: �Ӽ� �߰�


            dataList.structureDataList.Add(strucData);
        }
    }

    /// <summary>
    /// ���� ����� Data, ������ ����Ÿ�� ���� �ϳ��� �ٿ��
    /// </summary>
    [Serializable]
    public class DataList
    {
        public List<UnitData> unitDataList = new List<UnitData>();
        public List<StructureData> structureDataList = new List<StructureData>();

        //TODO:: �Ŵ��� ����Ÿ ����
    }

    [Serializable]
    public class UnitData
    {
        //���� �ƴ� ��������(Class)�� InstanceID�� ����ȴ�.
        public Vector3 position;    //Vector ���尡��
        public UnitType unitType;   //Enum�� Int�� �����
        //TODO:: �Ӽ� �߰�
    }

    [Serializable]
    public class StructureData
    {
        public Vector3 position;
        //TODO:: �Ӽ� �߰�
    }
}
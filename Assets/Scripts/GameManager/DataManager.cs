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
        //bool prettyPrint : 기본 false, false 한줄로 나열해서 저장, true 여러줄에 "Key : Value"로 이쁘게 저장
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
            //TODO:: 속성 추가


            dataList.unitDataList.Add(uniData);
        }
        else if( _gameObject.GetComponent<Structure>())
        {
            //Structure struc = _gameObject.GetComponent<Structure>();
            StructureData strucData = new StructureData();
            strucData.position = _gameObject.transform.position;
            //TODO:: 속성 추가


            dataList.structureDataList.Add(strucData);
        }
    }

    /// <summary>
    /// 최종 저장될 Data, 저장할 데이타를 전부 하나로 바운딩
    /// </summary>
    [Serializable]
    public class DataList
    {
        public List<UnitData> unitDataList = new List<UnitData>();
        public List<StructureData> structureDataList = new List<StructureData>();

        //TODO:: 매니저 데이타 저장
    }

    [Serializable]
    public class UnitData
    {
        //값이 아닌 참조형식(Class)은 InstanceID가 저장된다.
        public Vector3 position;    //Vector 저장가능
        public UnitType unitType;   //Enum은 Int로 저장됨
        //TODO:: 속성 추가
    }

    [Serializable]
    public class StructureData
    {
        public Vector3 position;
        //TODO:: 속성 추가
    }
}

using EnumManager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    //고객,스탭 프리펩
    public GameObject customerPrefab;
    public GameObject staffPrefab;
    
    //종족 Stat의 Max,Min값 저장 DB
    public Dictionary<Tribe, Dictionary<StatBind, int>> tribe { get; private set; }

    //스탭 관리
    int uniqueIndex;
    public List<Staff> staffList { get; private set; }  //스탭 리스트
    Queue<int> indexQue;    //안쓰는 이전 인덱스 재활용

    //리스폰 변수
    Transform[] respawn;
    float respawnTime;
    float maxRespawnTime = 2f;
   
    void Awake()
    {
        instance = this;
        tribe = new Dictionary<Tribe, Dictionary<StatBind, int>>();
        staffList = new List<Staff>();
        indexQue = new Queue<int>();
        respawn = transform.GetChild(1).GetComponentsInChildren<Transform>();

        ReadTribeDBToCSV();
    }

    void Update()
    {
        respawnTime += Time.deltaTime;
        if (respawnTime > maxRespawnTime)
        {
            CustomerGenerator();
            respawnTime = 0;
        }
    }

    /// <summary>
    /// 종족 DB
    /// </summary>
    void ReadTribeDBToCSV() //CSV 파일을 읽어서 종족 DB 초기화
    {
        var list = new List<Dictionary<string, object>>();
        list = CSVReader.Read("Document/AnotherWorldMarketTribe");

        for (int i = 0; i < list.Count; i++)
        {
            Tribe tri = (Tribe)Enum.Parse(typeof(Tribe), (string)list[i]["Tribe"]); //string을 Enum.Tribe로 변경
            tribe.Add(tri, new Dictionary<StatBind, int>());
            foreach (KeyValuePair<string, object> pair in list[i])
            {
                if (pair.Key == "Tribe") continue;  //첫번째 Key는 "Tribe"이므로 제외. StatBind에는 "Tribe"가 없다.
                StatBind statBind = (StatBind)Enum.Parse(typeof(StatBind), pair.Key); //string을 Enum.StatBind로 변경
                tribe[tri].Add(statBind, (int)pair.Value);
            }
        }
    }


    /// <summary>
    /// 스탭 관리
    /// </summary>
    public int GetUniqueIndex()
    {
        int index;
        if (indexQue.Count > 0)
            index = indexQue.Dequeue();
        else
            index = uniqueIndex++;
        
        return index;
    }
    
    public void AddStaff(Staff staff)
    {
        staffList.Add(staff);
    }

    public void RemoveStaff(Staff staff)
    {
        indexQue.Enqueue(staff.uniIndex);
        staffList.Remove(staff);
    }


    /// <summary>
    /// 리스폰
    /// </summary>
    public Vector2 GetRespawn() //리스폰 지역
    {
        return respawn[Random.Range(1, respawn.Length)].position;
    }

    //고객 생성
    void CustomerGenerator()
    {
        if (SceneManager.sceneCount == 1)
        {
            Instantiate(customerPrefab, respawn[Random.Range(1, respawn.Length)]);
        }
    }

    //TEST, 버튼사용, 스탭생성 //TODO:: 직원은 일정시간마다 자동 생성되어 패널에 보여진 후 선택하여 생성
    public void StaffGenerator()
    {
        Instantiate(staffPrefab, respawn[Random.Range(1, respawn.Length)]);
    }
}

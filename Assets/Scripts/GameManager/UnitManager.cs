using EnumManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public Dictionary<Tribe, Dictionary<StatBind, int>> tribe;

    public GameObject customerPrefab;
    public GameObject staffPrefab;


    Transform[] respawn;

    float respawnTime;
    float maxRespawnTime = 2f;
    void Awake()
    {
        tribe = new Dictionary<Tribe, Dictionary<StatBind, int>>();
        instance = this;
        respawn = transform.GetChild(1).GetComponentsInChildren<Transform>();

        ReadTribeDBToCSV();
    }

    void Update()
    {
        respawnTime += Time.deltaTime;
        if(respawnTime > maxRespawnTime)
        {
            CustomerGenerator();
            respawnTime = 0;
        }
    }

    void ReadTribeDBToCSV()
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

    public Vector2 GetRespawn()
    {
        return respawn[Random.Range(1, respawn.Length)].position;
    }
    

    void CustomerGenerator()
    {
        if (SceneManager.sceneCount == 1)
        {
            Instantiate(customerPrefab, respawn[Random.Range(1, respawn.Length)]);
        }
    }

    //TEST, 버튼사용
    public void StaffGenerator()
    {
        Instantiate(staffPrefab, respawn[Random.Range(1, respawn.Length)]);
    }
}

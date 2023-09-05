using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    
    public GameObject customerPrefab;
    public GameObject staffPrefab;


    Transform[] respawn;

    float respawnTime;
    float maxRespawnTime = 2f;
    void Awake()
    {
        instance = this;
        respawn = transform.GetChild(1).GetComponentsInChildren<Transform>();
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

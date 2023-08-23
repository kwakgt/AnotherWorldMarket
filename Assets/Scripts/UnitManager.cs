using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    Transform[] respawn;
    void Awake()
    {
        instance = this;
        respawn = transform.GetChild(0).gameObject.GetComponentsInChildren<Transform>();
    }

    public Vector2 GetRespawn()
    {
        return respawn[Random.Range(1, respawn.Length)].position;
    }
}

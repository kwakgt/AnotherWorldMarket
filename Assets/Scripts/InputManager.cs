using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    static InputManager instance;

    void Awake()
    {
        instance = this;
    }

    //TODO:: 단축키 추가
}

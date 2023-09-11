using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    //키보드
    public bool bKeyDown { get; set; }
    public bool eKeyDown { get; set; }
    public bool fKeyDown { get; set; }
    public bool iKeyDown { get; set; }
    public bool rKeyDown { get; set; }
    public bool spaceKeyDown { get; set; }
    public bool tabKeyDown { get; set; }


    //마우스
    public float scrollwheel { get; private set; }
    public float mouseX { get; private set; }   //-1이면 왼쪽, 1이면 오른쪽
    public float mouseY { get; private set; }   //-1이면 아래, 1이면 위
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        SetInputKey();
        SetMouseMovement();
    }

    void SetInputKey()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            bKeyDown = true;
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            eKeyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            fKeyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            iKeyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rKeyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            spaceKeyDown = true;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabKeyDown = true;
        }
        //TODO:: 단축키 추가
    }

    void SetMouseMovement()
    {
        scrollwheel = Input.GetAxis("Mouse ScrollWheel");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}

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
        bKeyDown = Input.GetKeyDown(KeyCode.B);
        eKeyDown = Input.GetKeyUp(KeyCode.E);
        fKeyDown = Input.GetKeyDown(KeyCode.F);
        iKeyDown = Input.GetKeyDown(KeyCode.I);
        rKeyDown = Input.GetKeyDown(KeyCode.R);
        spaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        tabKeyDown = Input.GetKeyDown(KeyCode.Tab);
        //TODO:: 단축키 추가
    }

    void SetMouseMovement()
    {
        scrollwheel = Input.GetAxis("Mouse ScrollWheel");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}

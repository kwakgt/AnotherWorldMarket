using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool bKeyDown { get; set; }
    public bool rKeyDown { get; set; }
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        InputKey();
    }



    void InputKey()
    {
  
        if (Input.GetKeyDown(KeyCode.R))
        {
            rKeyDown = true;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            bKeyDown = true;
        }
    }

    //TODO:: 단축키 추가
}

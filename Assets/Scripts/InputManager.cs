using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool rKeyDown { get; private set; }
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        InputButton();
    }



    void InputButton()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            rKeyDown = true;
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            GameManager.instance.ChangeBuliderMode();
        }
    }

    //TODO:: 단축키 추가
}

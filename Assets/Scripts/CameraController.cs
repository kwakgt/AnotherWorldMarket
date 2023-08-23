using UnityEngine;


public class CameraController : MonoBehaviour
{
    Camera cam;
    int speed = 10;
    float maxSize = 15f;
    float minSize = 5f;
    
    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    
    void Update()
    {
        MoveCamera();
        ChangeCameraSize();
    }

    void MoveCamera()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * speed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * speed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * speed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * speed * Time.unscaledDeltaTime);
        }
    }

    void ChangeCameraSize()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * speed, minSize, maxSize);
    }
}

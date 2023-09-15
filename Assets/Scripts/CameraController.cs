using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class CameraController : MonoBehaviour
{
    Camera cam;
    Camera minimap;
    int moveSpeed = 30;
    int zoomSpeed = 15;
    float zoomMax = 20f;
    float zoomMin = 5f;
    int gridIndex;
    

    Vector2 mouseXY;        //마우스 X,Y 움직임 감지, -1이면 왼쪽/아래, 1이면 오른쪽/위
    Vector2 cameraXY;       //카메라 중심점으로부터의 X,Y거리
    Vector2 mapSize;        //그리드 크기, 맵 크기와 그리드 크기는 같다
    Vector2 mapPosition;     //그리드 월드포지션
    void Awake()
    {
        mouseXY = new Vector2();
        cameraXY = new Vector2();
        cam = GetComponent<Camera>();
        minimap = GameObject.Find("MinimapCamera").GetComponent<Camera>();
    }

    void Start()
    {
        mapSize = Nodefinding.instance.GetGridWorldSize(gridIndex);
        mapPosition = Nodefinding.instance.GetGridCenterPosition(gridIndex);
    }

    void Update()
    {
        SetHalfSize();
        ChangeSize();
        Move();
        Teleport();
        InBounds();
    }

    void Move()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * moveSpeed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * moveSpeed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * moveSpeed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * moveSpeed * Time.unscaledDeltaTime);
        }
        if(Input.GetMouseButton(2))
        {
            mouseXY.x = InputManager.instance.mouseX;
            mouseXY.y = InputManager.instance.mouseY;
            transform.Translate(-mouseXY * moveSpeed * Time.unscaledDeltaTime * 5);
        }
        if (GameManager.instance.selectedUnit != null)
        {
            ChangeIndex(GameManager.instance.selectedUnit.gridIndex);
            transform.position = GameManager.instance.selectedUnit.transform.position + new Vector3(0, 0, -10);
        }
    }

    void ChangeSize()
    {
        if (InputManager.instance.scrollwheel != 0 && UIManager.instance.IsAllOff)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + InputManager.instance.scrollwheel * zoomSpeed, zoomMin, zoomMax);
    }

    void SetHalfSize()
    {
        //orthographicSize: 카메라 중앙에서 Y축 끝지점까지의 거리, height
        //widtgh : height = Screen.width : Screen.height
        cameraXY.y = cam.orthographicSize;
        cameraXY.x = cameraXY.y * Screen.width / Screen.height;
    }

    void Teleport()
    {
        if(InputManager.instance.tabKeyDown)
        {
            if(gridIndex == 0)
            {
                ChangeIndex(1);
                transform.position = (Vector3)Nodefinding.instance.GetGridCenterPosition(gridIndex) + new Vector3(0,0,-10);
                minimap.transform.position = transform.position;
            }
            else if(gridIndex == 1)
            {
                ChangeIndex(0);
                transform.position = (Vector3)Nodefinding.instance.GetGridCenterPosition(gridIndex) + new Vector3(0, 0, -10);
                minimap.transform.position = transform.position;
            }
        }
    }

    void ChangeIndex(int _gridIndex)
    {
        gridIndex = _gridIndex;
        mapSize = Nodefinding.instance.GetGridWorldSize(_gridIndex);
        mapPosition = Nodefinding.instance.GetGridCenterPosition(_gridIndex);
    }

    void InBounds()
    {
        //카메라와 월드맵 중심점이 같아야 한다.
        //카메라 경계
        // position.x - (mapSize.x/2) < width < position.x + (mapSize.x/2)
        // position.y - (mapSize.y/2) < height < position.y + (mapSize.y/2)
        float x = transform.position.x; //현재 위치의 X
        float y = transform.position.y; //현재 위치의 y

        //                  x - cameraXY.x : 카메라 최좌단 x
        //                  x + cameraXY.x : 카메라 최우단 x
        //                  y - cameraXY.y : 카메라 최하단 y
        //                  y + cameraXY.y : 카메라 최상단 y
        //  mapPostion.x - (mapSize.x / 2) : 맵 최좌단 x
        //  mapPostion.x + (mapSize.x / 2) : 맵 최우단 x
        //  mapPostion.y - (mapSize.y / 2) : 맵 최하단 y
        //  mapPostion.y + (mapSize.y / 2) : 맵 최상단 y
        
        //상하좌우
        if (x - cameraXY.x < mapPosition.x - (mapSize.x / 2))
        {
            transform.position = new Vector3(mapPosition.x - (mapSize.x / 2) + cameraXY.x, y, -10);
        }
        if (x + cameraXY.x > mapPosition.x + (mapSize.x / 2))
        {
            transform.position = new Vector3(mapPosition.x + (mapSize.x / 2) - cameraXY.x, y, -10);
        }
        if (y - cameraXY.y < mapPosition.y - (mapSize.y / 2))
        {
            transform.position = new Vector3(x, mapPosition.y - (mapSize.y / 2) + cameraXY.y, -10);
        }
        if (y + cameraXY.y > mapPosition.y + (mapSize.y / 2))
        {
            transform.position = new Vector3(x, mapPosition.y + (mapSize.y / 2) - cameraXY.y, -10);
        }

        //TODO :: 대각선
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Warehouse : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool displayGridGizmos;

    int index = 1;                             //창고 고유번호
    Vector2 worldPosition;                     //창고 월드포인트
    int frontSize;                             //창고 판매위치 노드크기
    int width;                                 //창고 노드너비
    int height;                                //창고 노드높이
    Direction frontDir;                         //창고 입구 방향

    Node[,] nodeOccupiedByWarehouse;
    Vector2[] frontPosition;

    SpriteRenderer frontRenderer;
    SpriteRenderer thisRenderer;

    int maxInvenSize;
    Item[] inventory;

    bool isMoving;
    float nodeRadius;                           //노드 반지름
    float nodeDiameter;                         //노드 지름

    void Awake()
    {
        frontSize = 4;
        height = 4;
        width = 4;
        worldPosition = transform.position;
        frontDir = Direction.Left;
        thisRenderer = GetComponent<SpriteRenderer>();
        frontRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        inventory = new Item[maxInvenSize];
    }

    void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        nodeOccupiedByWarehouse = SetNodeOccupiedByShelf(worldPosition, width, height);    //SetShelfFrontPosition(); 이전에 실행되야함,  순서중요!!!
        frontPosition = SetFrontPosition(nodeOccupiedByWarehouse);           //SetNodeOccupiedByShelf();가 먼저 실행되야함, nodeOccupiedByShelf값이 있어야 ShelfFrontPosition 계산 가능
    }

    void Update()
    {
        OnMoving();
    }

    Node[,] SetNodeOccupiedByShelf(Vector2 center, int sizeX, int sizeY) //매대가 점유하고 있는 노드들, 매대의 중심점, 크기
    {
        Vector2[,] vector2s = new Vector2[sizeX, sizeY];

        //dx = sizeX / 2 * (nodeRadius * 2) - nodeRadius
        float dx = sizeX * nodeRadius - nodeRadius;               //중심점x와 가장자리 노드의 중심점x의 차이
        float dy = sizeY * nodeRadius - nodeRadius;               //중심점y와 가장자리 노드의 중심점y의 차이

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vector2s[x, y] = new Vector2(center.x + (x * nodeDiameter) - dx, center.y + (y * nodeDiameter) - dy);   //중심좌표를 이용해서 점유하고 있는 노드의 월드좌표를 구한다.
            }
        }

        return Nodefinding.instance.RequestNodeArea(vector2s, sizeX, sizeY);    //월드 좌표를 노드로 변경
    }

    Vector2[] SetFrontPosition(Node[,] occupiedNodes)    //판매위치의 월드좌표(판매위치는 매대 앞)
    {
        Vector2[] position = new Vector2[frontSize];

        switch (frontDir)
        {
            case Direction.Left:
                for (int i = 0; i < frontSize; i++)    //가장 왼쪽줄의 worldpoint - 노드지름 
                    position[i] = new Vector2(occupiedNodes[0, i].worldPosition.x - nodeDiameter, occupiedNodes[0, i].worldPosition.y);
                break;
            case Direction.Right:
                for (int i = 0; i < frontSize; i++)    //가장 오른쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(occupiedNodes[width - 1, i].worldPosition.x + nodeDiameter, occupiedNodes[width - 1, i].worldPosition.y);
                break;
            case Direction.Up:
                for (int i = 0; i < frontSize; i++)    //가장 위쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(occupiedNodes[i, height - 1].worldPosition.x, occupiedNodes[i, height - 1].worldPosition.y + nodeDiameter);
                break;
            case Direction.Down:
                for (int i = 0; i < frontSize; i++)    //가장 아래줄의 worldpoint - 노드지름
                    position[i] = new Vector2(occupiedNodes[i, 0].worldPosition.x, occupiedNodes[i, 0].worldPosition.y - nodeDiameter);
                break;
        }
        return position;
    }

    public Vector2 GetWarehouseFrontPosition(int index = -1)
    {
        if (index < 0 || index >= frontSize) return frontPosition[Random.Range(0, frontSize)];
        return frontPosition[index];
    }

    void OnMoving()     //판매대 이동,회전,설치
    {
        if (isMoving && GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            //색변경
            frontRenderer.color = Color.Lerp(Color.white, Color.green, 0.5f);
            thisRenderer.color = Color.Lerp(Color.white, Color.blue, 0.5f);

            //이동, worldposition 변수가 아닌 transform 현재위치로 계산해야됨
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //마우스포지션 저장
            int dx = Mathf.RoundToInt(mousePosition.x - transform.position.x);              //마우스 위치와 처음위치의 x변화량
            int dy = Mathf.RoundToInt(mousePosition.y - transform.position.y);              //마우스 위치와 처음위치의 y변화량
            transform.Translate(dx, dy, 0, Space.World);                                    //변화량만큼 이동, (int)를 이용해 정수칸씩 이동, Space.World 필수

            //회전
            if (InputManager.instance.rKeyDown)
            {
                Debug.Log("R눌림");
                transform.Rotate(Vector3.forward, 90);                                      //90도 회전
                ChangeSaleDirection();                                                      //판매방향 변경
                SwapWidthAndHeight();                                                       //노드 가로,세로 크기변경
                InputManager.instance.rKeyDown = false;
            }

            //설치
            if (Input.GetMouseButtonDown(0))
            {
                //이동중에 마우스 클릭하면 현재 위치에 설치시도
                //설치 :: 설치할 위치에서 점유노드, 판매노드가 전부 walkable이면 설치
                //취소 :: 설치할 위치에서 점유노드, 판매노드 중에 unwalkable이 하나라도 있으면 취소

                Node[,] occupiedNodes = SetNodeOccupiedByShelf(transform.position, width, height); //현재 위치의 점유노드 계산
                Vector2[] frontPosition = SetFrontPosition(occupiedNodes);                                              //현재 위치의 판매위치 계산


                bool frontcheck = false;
                for (int i = 0; i < frontSize; i++)
                {
                    Node checkFrontNode = Nodefinding.instance.RequestNode(frontPosition[i]);   //판매위치로 노드위치 계산
                    if (checkFrontNode != null && checkFrontNode.walkable)
                    {
                        frontcheck = true;
                    }
                    else
                    {
                        frontcheck = false;
                        break;
                    }
                }

                bool walkableCheck = false;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Node checkOccupiedNode = occupiedNodes[x, y];
                        if (checkOccupiedNode != null && checkOccupiedNode.walkable)    //이동이 아닌 새로 설치할 때 점유노드가 없으므로 Null체크
                        {
                            walkableCheck = true;
                            checkOccupiedNode.walkable = false;                 //매대는 장애물이므로 walkable false로 변경
                        }
                        else
                        {
                            walkableCheck = false;                      //노드가 없거나 하나라도 unwalkable일 시 설치 취소
                            break;
                        }
                    }
                }

                if (walkableCheck && frontcheck)                      //노드가 전부 walkable이라면 설치시작
                {
                    worldPosition = transform.position;     //월드포지션 변경
                    nodeOccupiedByWarehouse = occupiedNodes;  //점유노드 변경
                    this.frontPosition = frontPosition;     //판매위치 변경
                }
                else
                {
                    ChangeWalkebleNodeOccupiedbyWarehouse(false);   //취소되면 처음에 walkable → true로 변경한 거 원상복구
                }
                isMoving = false;                       //설치 or 취소가 완료되면 이동완료

                //색 원상복구
                frontRenderer.color = Color.clear;
                thisRenderer.color = Color.white;
            }
        }
    }
    void ChangeWalkebleNodeOccupiedbyWarehouse(bool walkable) //노드들의 walkable 변경
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (nodeOccupiedByWarehouse[x, y] != null) nodeOccupiedByWarehouse[x, y].walkable = walkable;   //이동이 아닌 새로 설치할 때 점유노드가 없으므로 NULL체크
                else break;
            }
        }
    }

    void ChangeSaleDirection()
    {
        if (frontDir == Direction.Up)
        {
            frontDir = Direction.Left;
            return;
        }
        frontDir += 1;
    }

    void SwapWidthAndHeight()
    {
        int swap = width;
        width = height;
        height = swap;
    }

    //pointerDrag : GameObject 호출
    //dragging : 현재 드래그 작업이 진행 중입니다.
    //delta : 드래그 위치부터의 위치변화량
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
        if (GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            isMoving = true;    //이동모드로 전환
            //드래그로 들어올려진 순간 현재 점유하고 있는 노드를 walkable로 변경
            ChangeWalkebleNodeOccupiedbyWarehouse(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 끝");
    }

    void OnDrawGizmos()
    {
        if (nodeOccupiedByWarehouse != null && frontPosition != null && displayGridGizmos)
        {
            foreach (Node n in nodeOccupiedByWarehouse)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .5f));
            }

            foreach (Vector2 n in frontPosition)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(n, Vector3.one * (nodeDiameter - .5f));
            }
        }
    }

    enum Direction { Left, Down, Right, Up }
}

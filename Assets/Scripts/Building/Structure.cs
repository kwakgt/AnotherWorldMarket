using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

//TimeScale = 0 일 때, 충돌감지 안됨
//리지디바디 Kinematic 설정을 해야 이동할 때 콜라이더도 같이 이동된다. -> 다시 해보니 없어도 잘됨...뭐지..
public class Structure : MonoBehaviour, IBeginDragHandler, IDragHandler  //UI가 아니면 카메라에 Physics2DRaycater 컴포넌트 필요
{
    public bool displayGridGizmos;

    protected Vector2 worldPosition;                 //중심점 월드포인트
    protected int uniIndex;                          //고유번호
    bool isMoving;                         //이동 플래그
    bool isNewStructure;                        //새건물 플래그
    int countRotation;                     //회전수(건물 건설시 원상복구할때 쓰인다)
    int gridIndex;                         //현재 속해있는 그리드 인덱스

    float nodeRadius;                      //노드 반지름
    float nodeDiameter;                    //노드 지름
    protected int width;                             //노드너비
    protected int height;                            //노드높이
    Node[,] occupiedNodes;                 //점유하고 있는 노드

    protected int frontSize;                         //전방 크기
    Direction frontDir;                    //전방 방향
    protected Vector2[] frontPositions;              //전방 위치

    SpriteRenderer thisRenderer;           //이동중 색변경
    SpriteRenderer frontRenderer;          //이동중 색변경

    protected virtual void Awake()
    {
        width = (int)transform.localScale.x;
        height = (int)transform.localScale.y;
        frontSize = (int)MathF.Max(width, height);
        worldPosition = transform.position;
        frontDir = SetDirection();
        thisRenderer = GetComponent<SpriteRenderer>();
        frontRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        frontRenderer.color = Color.clear;
    }

    protected virtual void Start()
    {
        gridIndex = Nodefinding.instance.GetGridIndex(transform.position);
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        occupiedNodes = SetOccupiedNodes(worldPosition, width, height);    //SetFrontPosition(); 이전에 실행되야함,  순서중요!!!
        frontPositions = SetFrontPosition(occupiedNodes);
    }

    private void Update()
    {
        //TEST
        OnMoving();

        //Debug.Log(((ItemSlot[0] != null) ? ItemSlot[0].amount : 0) + "  ,  " + ((ItemSlot[1] != null) ? ItemSlot[1].amount : 0) + "  ,  " + ((ItemSlot[2] != null) ? ItemSlot[2].amount : 0) + "  ,  " + ((ItemSlot[3] != null) ? ItemSlot[3].amount : 0));
    }

    Node[,] SetOccupiedNodes(Vector2 center, int sizeX, int sizeY) //매대가 점유하고 있는 노드들, 매대의 중심점, 크기
    {
        Vector2[,] vector2s = new Vector2[sizeX, sizeY];

        //dx = sizeX / 2 * (nodeRadius * 2) - nodeRadius
        float dx = sizeX * nodeRadius - nodeRadius;               //중심점x와 가장자리 노드의 중심점x의 차이
        float dy = sizeY * nodeRadius - nodeRadius;               //중심점y와 가장자리 노드의 중심점y의 차이

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vector2s[x, y] = new Vector2(center.x + (x * nodeDiameter) - dx, center.y + (y * nodeDiameter) - dy);   //매대의 중심점을 이용해서 점유하고 있는 노드의 월드좌표를 구한다.
            }
        }

        return Nodefinding.instance.RequestNodeArea(vector2s, sizeX, sizeY, gridIndex);    //월드 좌표를 노드로 변경
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

    public Vector2 GetFrontPosition(int index = -1)
    {
        if (index < 0 || index >= frontSize) return frontPositions[Random.Range(0, frontSize)];
        return frontPositions[index];
    }

    public int FindIndexOfFrontPosition(Vector2 _frontPosition)
    {
        //False 시 -1을 반환한다.
        return Array.IndexOf(frontPositions, _frontPosition);
    }

    public void OnMoving()     //판매대 이동,회전,설치
    {
        if (isMoving && GameManager.instance.CompareTo(GameManager.GameMode.Builder))
        {
            //색변경
            thisRenderer.color = new Color(0, 0, 100, 100);
            frontRenderer.color = Color.white;

            //이동, worldposition 변수가 아닌 transform 현재위치로 계산해야됨
            Vector2 firstPosition = worldPosition;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //마우스포지션 저장
            int dx = Mathf.RoundToInt(mousePosition.x - transform.position.x);              //마우스 위치와 처음위치의 x변화량
            int dy = Mathf.RoundToInt(mousePosition.y - transform.position.y);              //마우스 위치와 처음위치의 y변화량
            transform.Translate(dx, dy, 0, Space.World);                                    //변화량만큼 이동, (int)를 이용해 정수칸씩 이동, Space.World 필수

            //회전
            if (InputManager.instance.rKeyDown)
            {
                InputManager.instance.rKeyDown = false;
                Rotate();
            }

            //설치
            if (Input.GetMouseButtonDown(0))
            {

                //이동중에 마우스 클릭하면 현재 위치에 설치시도
                //설치 :: 설치할 위치에서 점유노드, 판매노드가 전부 walkable이면 설치
                //취소 :: 설치할 위치에서 점유노드, 판매노드 중에 unwalkable이 하나라도 있으면 취소

                Node[,] tempOccupiedNodes = SetOccupiedNodes(transform.position, width, height); //현재 위치의 점유노드 계산
                Debug.Log(transform.position);
                Debug.Log(tempOccupiedNodes[0, 0].worldPosition);
                Vector2[] tempFrontPosition = SetFrontPosition(tempOccupiedNodes);                                              //현재 위치의 판매위치 계산


                bool frontWalkablecheck = false;    //설치할 front노드의 장애물 체크, 장애물이 있으면 false, 없으면 true
                for (int i = 0; i < frontSize; i++)
                {
                    Node checkFrontNode = Nodefinding.instance.RequestNode(tempFrontPosition[i], gridIndex);   //판매위치로 노드위치 계산
                    if (checkFrontNode != null && checkFrontNode.walkable)  //설치할 front 노드에 장애물이 없다면
                    {
                        frontWalkablecheck = true;
                    }
                    else
                    {
                        frontWalkablecheck = false;
                        break;
                    }
                }

                bool walkableCheck = false;     //설치할 점유노드의 장애물 체크, 장애물이 있으면 false, 없으면 true
                bool frontCheck = true;         //설치할 점유노드가 다른 건물의 front 노드인지 체크, front노드이면 false, front노드가 아니면 true
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Node checkOccupiedNode = tempOccupiedNodes[x, y];

                        //노드마다 레이캐스트 하여 Front 걸러내기
                        RaycastHit2D hit;
                        hit = Physics2D.Raycast(checkOccupiedNode.worldPosition, Vector2.zero, 1, LayerMask.GetMask("Front"));  //이상하게 레이어 없이 감지하면 Front가 감지 안됨...RaycastAll로도 안됨..
                        if (hit)
                        {
                            frontCheck = false;
                            break;
                        }

                        if (checkOccupiedNode != null && checkOccupiedNode.walkable)    //이동이 아닌 새로 설치할 때 점유노드가 없으므로 Null체크
                        {
                            walkableCheck = true;
                        }
                        else
                        {
                            walkableCheck = false;                      //노드가 없거나 하나라도 unwalkable일 시 설치 취소
                            break;
                        }
                    }

                    if (!walkableCheck || !frontCheck) break;  //false면 빠져나오기
                }

                if (walkableCheck && frontWalkablecheck && frontCheck)                      //노드가 전부 walkable이고, 구조체 입구와 충돌이 없으면 설치
                {
                    worldPosition = transform.position;     //월드포지션 변경
                    occupiedNodes = tempOccupiedNodes;      //점유노드 변경
                    frontPositions = tempFrontPosition;     //전방위치 변경
                    ChangeWalkableOfOccupiedNode(false);
                    countRotation = 0;
                    isNewStructure = false;
                    if (tag.Equals("Shelf"))
                    {
                        ShelfManager.instance.UpdateShelfDictionary(uniIndex, (Shelf)this);  //변경된 정보 매니저에 업데이트
                    }
                    //TODO:: 매니저에 업데이트 필요한 건물 추가
                }
                else if(isNewStructure)
                {
                    Destroy(gameObject);    //새 구조체인데 건설에 실패했으면 삭제
                }
                else
                {
                    Rotate(countRotation);                      //회전 원상복구
                    transform.position = firstPosition;         //위치 원상복구, 회전이 먼저 복구되어야함, 순서중요
                    countRotation = 0;
                    ChangeWalkableOfOccupiedNode(false);   //취소되면 처음에 walkable → true로 변경한 거 원상복구
                }
                isMoving = false;                       //설치 or 취소가 완료되면 이동완료

                //색 원상복구
                thisRenderer.color = Color.white;
                frontRenderer.color = Color.clear;

            }
        }
    }

    void ChangeWalkableOfOccupiedNode(bool walkable) //노드들의 walkable 변경
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (occupiedNodes[x, y] != null) occupiedNodes[x, y].walkable = walkable;   //이동이 아닌 새로 설치할 때 점유노드가 없으므로 NULL체크
                else break;
            }
        }
    }

    protected virtual void Rotate(int rewind = 3)
    {
        for (int i = 0; i < 4 - rewind; i++)
        {
            transform.Rotate(Vector3.forward, 90);                                      //90도 회전
            ChangeFrontDirection();                                                     //판매방향변경
            SwapWidthAndHeight();                                                       //노드 가로,세로 크기변경
            countRotation = (countRotation + 1) % 4;                 //회전수++
        }
    }

    protected void ChangeFrontDirection()
    {
        if (frontDir == Direction.Up)
        {
            frontDir = Direction.Left;
            return;
        }
        frontDir += 1;
    }

    protected void SwapWidthAndHeight()
    {
        int swap = width;
        width = height;
        height = swap;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.instance.CompareTo(GameManager.GameMode.Builder))
        {
            isMoving = true;    //이동모드로 전환
            //드래그로 들어올려진 순간 현재 점유하고 있는 노드를 walkable로 변경
            ChangeWalkableOfOccupiedNode(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    void OnDrawGizmos()
    {
        if (occupiedNodes != null && frontPositions != null && displayGridGizmos)
        {
            foreach (Node n in occupiedNodes)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .5f));
            }

            foreach (Vector2 n in frontPositions)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(n, Vector3.one * (nodeDiameter - .5f));
            }
        }
    }

    Direction SetDirection() //개발중에 초기화를 위해 수동 방향 조정, 90,270도에서 높이와 너비가 바뀌기 때문에
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        if (rotation == Vector3.zero)
            return Direction.Left;
        else if (rotation == new Vector3(0, 0, 90))
        {
            SwapWidthAndHeight();
            return Direction.Down;
        }
        else if (rotation == new Vector3(0, 0, 180))
            return Direction.Right;
        else
        {
            SwapWidthAndHeight();
            return Direction.Up;
        }
    }

    //건설패널에서 사용
    public bool IsMoving
    {
        get { return isMoving; }
        set { isMoving = value; }
    }

    public bool IsNewStructure
    {
        get { return isNewStructure; }
        set { isNewStructure = value; }
    }

    protected enum Direction { Left, Down, Right, Up }
}

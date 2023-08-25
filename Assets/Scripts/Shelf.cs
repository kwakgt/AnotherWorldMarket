using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

//리지디바디 Kinematic 설정을 해야 이동할 때 콜라이더도 같이 이동된다.
public class Shelf : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool displayGridGizmos;

    int shelfIndex;                             //매대 고유번호
    Vector2 worldPosition;                      //중심점 월드포인트
    int shelfFrontSize;                         //매대 판매위치 노드크기
    int shelfWidth;                             //매대 노드너비
    int shelfHeight;                             //매대 노드높이
    Direction saleDir;                          //현재 매대의 판매방향

    Node[,] nodeOccupiedByShelf;                //매대가 차지하고 있는 노드들
    Vector2[] ShelfFrontPosition;               //판매위치 노드의 중심좌표

    bool isMoving;                              //모드 플래그
    float nodeRadius;                           //노드 반지름
    float nodeDiameter;                         //노드 지름
    Item[] ItemSlot;                            //매대 아이템 슬롯

    void Awake()
    {
        shelfFrontSize = 4;
        shelfHeight = 4;
        shelfWidth = 2;
        worldPosition = transform.position;
        saleDir = Direction.Left;
        ItemSlot = new Item[shelfFrontSize];
    }
    // Start is called before the first frame update
    void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        shelfIndex = ShelfManager.instance.RequestShelfIndex();
        nodeOccupiedByShelf = SetNodeOccupiedByShelf(worldPosition, shelfWidth, shelfHeight);    //SetShelfFrontPosition(); 이전에 실행되야함,  순서중요!!!
        ShelfFrontPosition = SetShelfFrontPosition(nodeOccupiedByShelf);                                               //SetNodeOccupiedByShelf();가 먼저 실행되야함, nodeOccupiedByShelf값이 있어야 ShelfFrontPosition 계산 가능

        ShelfManager.instance.AddShelfDictionary(shelfIndex, this); //현재 매대를 매니저에 추가

        //TEST
        PutItemInSlot(0, 50);
        PutItemInSlot(1, 50);
        PutItemInSlot(2, 50);
        PutItemInSlot(3, 50);

    }

    private void Update()
    {
        //TEST
        OnMoving();
        
        //Debug.Log(((ItemSlot[0] != null) ? ItemSlot[0].amount : 0) + "  ,  " + ((ItemSlot[1] != null) ? ItemSlot[1].amount : 0) + "  ,  " + ((ItemSlot[2] != null) ? ItemSlot[2].amount : 0) + "  ,  " + ((ItemSlot[3] != null) ? ItemSlot[3].amount : 0));
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
                vector2s[x, y] = new Vector2(center.x + (x * nodeDiameter) - dx, center.y + (y * nodeDiameter) - dy);   //매대의 중심점을 이용해서 점유하고 있는 노드의 월드좌표를 구한다.
            }
        }

        return Nodefinding.instance.RequestNodeArea(vector2s, sizeX, sizeY);    //월드 좌표를 노드로 변경
    }

    Vector2[] SetShelfFrontPosition(Node[,] occupiedNodes)    //판매위치의 월드좌표(판매위치는 매대 앞)
    {
        Vector2[] position = new Vector2[shelfFrontSize];

        switch (saleDir)
        {
            case Direction.Left:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 왼쪽줄의 worldpoint - 노드지름 
                    position[i] = new Vector2(occupiedNodes[0, i].worldPosition.x - nodeDiameter, occupiedNodes[0, i].worldPosition.y);
                break;
            case Direction.Right:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 오른쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(occupiedNodes[shelfWidth - 1, i].worldPosition.x + nodeDiameter, occupiedNodes[shelfWidth - 1, i].worldPosition.y);
                break;
            case Direction.Up:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 위쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(occupiedNodes[i, shelfHeight - 1].worldPosition.x, occupiedNodes[i, shelfHeight - 1].worldPosition.y + nodeDiameter);
                break;
            case Direction.Down:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 아래줄의 worldpoint - 노드지름
                    position[i] = new Vector2(occupiedNodes[i, 0].worldPosition.x, occupiedNodes[i, 0].worldPosition.y - nodeDiameter);
                break;
        }
        return position;
    }

    public Vector2 GetShelfFrontPosition(int index = -1)
    {
        if (index < 0 || index >= shelfFrontSize) return ShelfFrontPosition[Random.Range(0, shelfFrontSize)];
        return ShelfFrontPosition[index];
    }

    public Item FindItemInSlot(Vector2 _shelfFrontPosition)   //선반 앞 위치로 매대 슬롯 가져오기
    {
        if (Array.IndexOf(ShelfFrontPosition, _shelfFrontPosition) >= 0)
            return ItemSlot[Array.IndexOf(ShelfFrontPosition, _shelfFrontPosition)];
        else
            return null;
    }

    public int FindItemSlotIndex(Vector2 _shelfFrontPosition)
    {
        return Array.IndexOf(ShelfFrontPosition, _shelfFrontPosition);
    }

    public bool EmptyItemSlot(int index)
    {
        if (ItemSlot[index].amount > 0) return false;
        ItemSlot[index] = null;
        return true;
    }

    void PutItemInSlot(int index, int amount)
    {
        if (ItemSlot[index] == null)
        {
            ItemSlot[index] = ItemManager.instance.GetRandomItem();
            ItemSlot[index].PlusAmount(amount); 
        }
    }


    void OnDrawGizmos()
    {
        if (nodeOccupiedByShelf != null && ShelfFrontPosition != null && displayGridGizmos)
        {
            foreach (Node n in nodeOccupiedByShelf)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .5f));
            }

            foreach (Vector2 n in ShelfFrontPosition)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(n, Vector3.one * (nodeDiameter - .5f));
            }
        }
    }

    

    void OnMoving()     //판매대 이동,회전,설치
    {
        if (isMoving && GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            //이동, worldposition 변수가 아닌 transform 현재위치로 계산해야됨
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //마우스포지션 저장
            int dx = (int)(mousePosition.x - transform.position.x);                         //마우스 위치와 처음위치의 x변화량
            int dy = (int)(mousePosition.y - transform.position.y);                         //마우스 위치와 처음위치의 y변화량
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
            if(Input.GetMouseButtonDown(0))
            {
                //이동중에 마우스 클릭하면 현재 위치에 설치시도
                //설치 :: 설치할 위치에서 점유노드, 판매노드가 전부 walkable이면 설치
                //취소 :: 설치할 위치에서 점유노드, 판매노드 중에 unwalkable이 하나라도 있으면 취소

                Node[,] occupiedNodes = SetNodeOccupiedByShelf(transform.position, shelfWidth, shelfHeight); //현재 위치의 점유노드 계산
                Vector2[] frontPosition = SetShelfFrontPosition(occupiedNodes);                                              //현재 위치의 판매위치 계산


                bool frontcheck = false;
                for (int i = 0; i < shelfFrontSize; i++)
                {
                    Node checkFrontNode = Nodefinding.instance.RequestNode(frontPosition[i]);   //판매위치로 노드위치 계산
                    if(checkFrontNode != null && checkFrontNode.walkable)
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
                for (int y = 0; y < shelfHeight; y++)
                {
                    for (int x = 0; x < shelfWidth; x++)
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
                    nodeOccupiedByShelf = occupiedNodes;  //점유노드 변경
                    ShelfFrontPosition = frontPosition;     //판매위치 변경
                    ShelfManager.instance.UpdateShelfDictionary(shelfIndex, this);  //변경된 정보 매니저에 업데이트
                }
                else
                {
                    ChangeWalkebleNodeOccupiedbyshelf(false);   //취소되면 처음에 walkable → true로 변경한 거 원상복구
                }
                isMoving = false;                       //설치 or 취소가 완료되면 이동완료
            }
        }
    }

    void ChangeWalkebleNodeOccupiedbyshelf(bool walkable) //노드들의 walkable 변경
    {
        for (int y = 0; y < shelfHeight; y++)
        {
            for (int x = 0; x < shelfWidth; x++)
            {
                if (nodeOccupiedByShelf[x, y] != null) nodeOccupiedByShelf[x, y].walkable = walkable;   //이동이 아닌 새로 설치할 때 점유노드가 없으므로 NULL체크
                else break;
            }
        }
    }

    void ChangeSaleDirection()
    {
        if(saleDir == Direction.Up)
        {
            saleDir = Direction.Left;
            return;
        }
        saleDir += 1;
    }

    void SwapWidthAndHeight()
    {
        int swap = shelfWidth;
        shelfWidth = shelfHeight;
        shelfHeight = swap;
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
            ChangeWalkebleNodeOccupiedbyshelf(true);
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

    enum Type { SmallShelf, MediumShelf, LargeShelf }
    enum Direction { Left, Down, Right, Up }

}

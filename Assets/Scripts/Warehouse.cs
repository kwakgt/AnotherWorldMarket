using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Warehouse : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool displayGridGizmos;

    int index = 1;                              //â�� ������ȣ
    Vector2 worldPosition;                      //â�� ��������Ʈ
    int frontSize;                              //â�� �Ǹ���ġ ���ũ��
    int width;                                  //â�� ���ʺ�
    int height;                                 //â�� ������
    Direction frontDir;                         //â�� �Ա� ����

    Node[,] nodeOccupiedByWarehouse;            //�������
    Vector2[] frontPosition;                    //���� ������(�Ա�)

    SpriteRenderer frontRenderer;               //�̵��� ������
    SpriteRenderer thisRenderer;                //�̵��� ������

    int maxInvenSize;                           //�ִ� �κ��丮
    Item[] inventory;                           //�κ��丮 ����

    bool isMoving;                              //�̵� �÷���
    float nodeRadius;                           //��� ������
    float nodeDiameter;                         //��� ����

    void Awake()
    {
        maxInvenSize = 40;
        frontSize = 4;
        height = 4;
        width = 4;
        worldPosition = transform.position;
        frontDir = Direction.Left;
        thisRenderer = GetComponent<SpriteRenderer>();
        frontRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        frontRenderer.color = Color.clear;
        inventory = new Item[maxInvenSize];
    }

    void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        nodeOccupiedByWarehouse = SetNodeOccupiedByShelf(worldPosition, width, height);    //SetShelfFrontPosition(); ������ ����Ǿ���,  �����߿�!!!
        frontPosition = SetFrontPosition(nodeOccupiedByWarehouse);           //SetNodeOccupiedByShelf();�� ���� ����Ǿ���, nodeOccupiedByShelf���� �־�� ShelfFrontPosition ��� ����
    }

    void Update()
    {
        OnMoving();
    }

    Node[,] SetNodeOccupiedByShelf(Vector2 center, int sizeX, int sizeY) //�Ŵ밡 �����ϰ� �ִ� ����, �Ŵ��� �߽���, ũ��
    {
        Vector2[,] vector2s = new Vector2[sizeX, sizeY];

        //dx = sizeX / 2 * (nodeRadius * 2) - nodeRadius
        float dx = sizeX * nodeRadius - nodeRadius;               //�߽���x�� �����ڸ� ����� �߽���x�� ����
        float dy = sizeY * nodeRadius - nodeRadius;               //�߽���y�� �����ڸ� ����� �߽���y�� ����

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vector2s[x, y] = new Vector2(center.x + (x * nodeDiameter) - dx, center.y + (y * nodeDiameter) - dy);   //�߽���ǥ�� �̿��ؼ� �����ϰ� �ִ� ����� ������ǥ�� ���Ѵ�.
            }
        }

        return Nodefinding.instance.RequestNodeArea(vector2s, sizeX, sizeY);    //���� ��ǥ�� ���� ����
    }

    Vector2[] SetFrontPosition(Node[,] occupiedNodes)    //�Ǹ���ġ�� ������ǥ(�Ǹ���ġ�� �Ŵ� ��)
    {
        Vector2[] position = new Vector2[frontSize];

        switch (frontDir)
        {
            case Direction.Left:
                for (int i = 0; i < frontSize; i++)    //���� �������� worldpoint - ������� 
                    position[i] = new Vector2(occupiedNodes[0, i].worldPosition.x - nodeDiameter, occupiedNodes[0, i].worldPosition.y);
                break;
            case Direction.Right:
                for (int i = 0; i < frontSize; i++)    //���� ���������� worldpoint + ������� 
                    position[i] = new Vector2(occupiedNodes[width - 1, i].worldPosition.x + nodeDiameter, occupiedNodes[width - 1, i].worldPosition.y);
                break;
            case Direction.Up:
                for (int i = 0; i < frontSize; i++)    //���� �������� worldpoint + ������� 
                    position[i] = new Vector2(occupiedNodes[i, height - 1].worldPosition.x, occupiedNodes[i, height - 1].worldPosition.y + nodeDiameter);
                break;
            case Direction.Down:
                for (int i = 0; i < frontSize; i++)    //���� �Ʒ����� worldpoint - �������
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

    void OnMoving()     //�ǸŴ� �̵�,ȸ��,��ġ
    {
        if (isMoving && GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            //������
            frontRenderer.color = Color.Lerp(Color.white, Color.green, 0.5f);
            thisRenderer.color = Color.Lerp(Color.white, Color.blue, 0.5f);

            //�̵�, worldposition ������ �ƴ� transform ������ġ�� ����ؾߵ�
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //���콺������ ����
            int dx = Mathf.RoundToInt(mousePosition.x - transform.position.x);              //���콺 ��ġ�� ó����ġ�� x��ȭ��
            int dy = Mathf.RoundToInt(mousePosition.y - transform.position.y);              //���콺 ��ġ�� ó����ġ�� y��ȭ��
            transform.Translate(dx, dy, 0, Space.World);                                    //��ȭ����ŭ �̵�, (int)�� �̿��� ����ĭ�� �̵�, Space.World �ʼ�

            //ȸ��
            if (InputManager.instance.rKeyDown)
            {
                Debug.Log("R����");
                transform.Rotate(Vector3.forward, 90);                                      //90�� ȸ��
                ChangeSaleDirection();                                                      //�ǸŹ��� ����
                SwapWidthAndHeight();                                                       //��� ����,���� ũ�⺯��
                InputManager.instance.rKeyDown = false;
            }

            //��ġ
            if (Input.GetMouseButtonDown(0))
            {
                //�̵��߿� ���콺 Ŭ���ϸ� ���� ��ġ�� ��ġ�õ�
                //��ġ :: ��ġ�� ��ġ���� �������, �Ǹų�尡 ���� walkable�̸� ��ġ
                //��� :: ��ġ�� ��ġ���� �������, �Ǹų�� �߿� unwalkable�� �ϳ��� ������ ���

                Node[,] occupiedNodes = SetNodeOccupiedByShelf(transform.position, width, height); //���� ��ġ�� ������� ���
                Vector2[] frontPosition = SetFrontPosition(occupiedNodes);                                              //���� ��ġ�� �Ǹ���ġ ���


                bool frontcheck = false;
                for (int i = 0; i < frontSize; i++)
                {
                    Node checkFrontNode = Nodefinding.instance.RequestNode(frontPosition[i]);   //�Ǹ���ġ�� �����ġ ���
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
                        if (checkOccupiedNode != null && checkOccupiedNode.walkable)    //�̵��� �ƴ� ���� ��ġ�� �� ������尡 �����Ƿ� Nullüũ
                        {
                            walkableCheck = true;
                            checkOccupiedNode.walkable = false;                 //�Ŵ�� ��ֹ��̹Ƿ� walkable false�� ����
                        }
                        else
                        {
                            walkableCheck = false;                      //��尡 ���ų� �ϳ��� unwalkable�� �� ��ġ ���
                            break;
                        }
                    }
                }

                if (walkableCheck && frontcheck)                      //��尡 ���� walkable�̶�� ��ġ����
                {
                    worldPosition = transform.position;     //���������� ����
                    nodeOccupiedByWarehouse = occupiedNodes;  //������� ����
                    this.frontPosition = frontPosition;     //�Ǹ���ġ ����
                }
                else
                {
                    ChangeWalkebleNodeOccupiedbyWarehouse(false);   //��ҵǸ� ó���� walkable �� true�� ������ �� ���󺹱�
                }
                isMoving = false;                       //��ġ or ��Ұ� �Ϸ�Ǹ� �̵��Ϸ�

                //�� ���󺹱�
                frontRenderer.color = Color.clear;
                thisRenderer.color = Color.white;
            }
        }
    }
    void ChangeWalkebleNodeOccupiedbyWarehouse(bool walkable) //������ walkable ����
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (nodeOccupiedByWarehouse[x, y] != null) nodeOccupiedByWarehouse[x, y].walkable = walkable;   //�̵��� �ƴ� ���� ��ġ�� �� ������尡 �����Ƿ� NULLüũ
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

    //pointerDrag : GameObject ȣ��
    //dragging : ���� �巡�� �۾��� ���� ���Դϴ�.
    //delta : �巡�� ��ġ������ ��ġ��ȭ��
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ����");
        if (GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            isMoving = true;    //�̵����� ��ȯ
            //�巡�׷� ���÷��� ���� ���� �����ϰ� �ִ� ��带 walkable�� ����
            ChangeWalkebleNodeOccupiedbyWarehouse(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("�巡�� ��");
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
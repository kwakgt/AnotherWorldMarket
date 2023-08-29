using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

//������ٵ� Kinematic ������ �ؾ� �̵��� �� �ݶ��̴��� ���� �̵��ȴ�.
public class Structure : MonoBehaviour,  IDragHandler    //UI�� �ƴϸ� ī�޶� Physics2DRaycater ������Ʈ �ʿ�
{
    public      bool displayGridGizmos;

    protected   Vector2 worldPosition;                 //�߽��� ��������Ʈ
    protected   int uniIndex;                          //������ȣ
                bool isMoving;                         //�̵� �÷���
                int countRotation;                     //ȸ����(�ǹ� �Ǽ��� ���󺹱��Ҷ� ���δ�)
    
                float nodeRadius;                      //��� ������
                float nodeDiameter;                    //��� ����
    protected   int width;                             //���ʺ�
    protected   int height;                            //������
                Node[,] occupiedNodes;                 //�����ϰ� �ִ� ���

    protected   int frontSize;                         //���� ũ��
                Direction frontDir;                    //���� ����
    protected   Vector2[] frontPositions;              //���� ��ġ

                SpriteRenderer thisRenderer;           //�̵��� ������
                SpriteRenderer frontRenderer;          //�̵��� ������
                Color firstColor;                      //���� �� ��

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
        firstColor = thisRenderer.color;
    }

    protected virtual void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        occupiedNodes = SetOccupiedNodes(worldPosition, width, height);    //SetFrontPosition(); ������ ����Ǿ���,  �����߿�!!!
        frontPositions = SetFrontPosition(occupiedNodes);
    }

    private void Update()
    {
        //TEST
        OnMoving();

        //Debug.Log(((ItemSlot[0] != null) ? ItemSlot[0].amount : 0) + "  ,  " + ((ItemSlot[1] != null) ? ItemSlot[1].amount : 0) + "  ,  " + ((ItemSlot[2] != null) ? ItemSlot[2].amount : 0) + "  ,  " + ((ItemSlot[3] != null) ? ItemSlot[3].amount : 0));
    }

    Node[,] SetOccupiedNodes(Vector2 center, int sizeX, int sizeY) //�Ŵ밡 �����ϰ� �ִ� ����, �Ŵ��� �߽���, ũ��
    {
        Vector2[,] vector2s = new Vector2[sizeX, sizeY];

        //dx = sizeX / 2 * (nodeRadius * 2) - nodeRadius
        float dx = sizeX * nodeRadius - nodeRadius;               //�߽���x�� �����ڸ� ����� �߽���x�� ����
        float dy = sizeY * nodeRadius - nodeRadius;               //�߽���y�� �����ڸ� ����� �߽���y�� ����

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                vector2s[x, y] = new Vector2(center.x + (x * nodeDiameter) - dx, center.y + (y * nodeDiameter) - dy);   //�Ŵ��� �߽����� �̿��ؼ� �����ϰ� �ִ� ����� ������ǥ�� ���Ѵ�.
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

    public Vector2 GetFrontPosition(int index = -1)
    {
        if (index < 0 || index >= frontSize) return frontPositions[Random.Range(0, frontSize)];
        return frontPositions[index];
    }

    public int FindIndexOfFrontPosition(Vector2 _frontPosition)
    {
        //False �� -1�� ��ȯ�Ѵ�.
        return Array.IndexOf(frontPositions, _frontPosition);
    }

    void OnMoving()     //�ǸŴ� �̵�,ȸ��,��ġ
    {
        if (isMoving && GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            //������
            frontRenderer.color = Color.Lerp(Color.white, Color.green, 0.5f);
            thisRenderer.color = Color.Lerp(Color.white, Color.blue, 0.5f);

            //�̵�, worldposition ������ �ƴ� transform ������ġ�� ����ؾߵ�
            Vector2 firstPosition = worldPosition;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //���콺������ ����
            int dx = Mathf.RoundToInt(mousePosition.x - transform.position.x);              //���콺 ��ġ�� ó����ġ�� x��ȭ��
            int dy = Mathf.RoundToInt(mousePosition.y - transform.position.y);              //���콺 ��ġ�� ó����ġ�� y��ȭ��
            transform.Translate(dx, dy, 0, Space.World);                                    //��ȭ����ŭ �̵�, (int)�� �̿��� ����ĭ�� �̵�, Space.World �ʼ�

            //ȸ��
            if (InputManager.instance.rKeyDown)
            {
                InputManager.instance.rKeyDown = false;
                Rotate();
            }

            //��ġ
            if (Input.GetMouseButtonDown(0))
            {
                //�̵��߿� ���콺 Ŭ���ϸ� ���� ��ġ�� ��ġ�õ�
                //��ġ :: ��ġ�� ��ġ���� �������, �Ǹų�尡 ���� walkable�̸� ��ġ
                //��� :: ��ġ�� ��ġ���� �������, �Ǹų�� �߿� unwalkable�� �ϳ��� ������ ���

                Node[,] tempOccupiedNodes = SetOccupiedNodes(transform.position, width, height); //���� ��ġ�� ������� ���
                Vector2[] tempFrontPosition = SetFrontPosition(tempOccupiedNodes);                                              //���� ��ġ�� �Ǹ���ġ ���


                bool frontWalkablecheck = false;
                for (int i = 0; i < frontSize; i++)
                {
                    Node checkFrontNode = Nodefinding.instance.RequestNode(tempFrontPosition[i]);   //�Ǹ���ġ�� �����ġ ���
                    if (checkFrontNode != null && checkFrontNode.walkable)
                    {
                        frontWalkablecheck = true;
                    }
                    else
                    {
                        frontWalkablecheck = false;
                        break;
                    }
                }

                bool walkableCheck = false;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Node checkOccupiedNode = tempOccupiedNodes[x, y];
                        if (checkOccupiedNode != null && checkOccupiedNode.walkable)    //�̵��� �ƴ� ���� ��ġ�� �� ������尡 �����Ƿ� Nullüũ
                        {
                            walkableCheck = true;
                        }
                        else
                        {
                            walkableCheck = false;                      //��尡 ���ų� �ϳ��� unwalkable�� �� ��ġ ���
                            break;
                        }
                    }

                    if (!walkableCheck) break;  //false�� ����������
                }

                if (walkableCheck && frontWalkablecheck)                      //��尡 ���� walkable�̶�� ��ġ����
                {
                    worldPosition = transform.position;     //���������� ����
                    occupiedNodes = tempOccupiedNodes;      //������� ����
                    frontPositions = tempFrontPosition;     //������ġ ����
                    ChangeWalkebleOfOccupiedNode(false);
                    countRotation = 0;
                    if (tag.Equals("Shelf"))
                    {
                        ShelfManager.instance.UpdateShelfDictionary(uniIndex, (Shelf)this);  //����� ���� �Ŵ����� ������Ʈ
                    }
                    //TODO:: �Ŵ����� ������Ʈ �ʿ��� �ǹ� �߰�
                }
                else
                {
                    Rotate(countRotation);                      //ȸ�� ���󺹱�
                    transform.position = firstPosition;         //��ġ ���󺹱�, ȸ���� ���� �����Ǿ����, �����߿�
                    countRotation = 0;
                    ChangeWalkebleOfOccupiedNode(false);   //��ҵǸ� ó���� walkable �� true�� ������ �� ���󺹱�
                }
                isMoving = false;                       //��ġ or ��Ұ� �Ϸ�Ǹ� �̵��Ϸ�

                //�� ���󺹱�
                frontRenderer.color = Color.clear;
                thisRenderer.color = firstColor;
            }
        }
    }

    void ChangeWalkebleOfOccupiedNode(bool walkable) //������ walkable ����
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (occupiedNodes[x, y] != null) occupiedNodes[x, y].walkable = walkable;   //�̵��� �ƴ� ���� ��ġ�� �� ������尡 �����Ƿ� NULLüũ
                else break;
            }
        }
    }

    protected virtual void Rotate(int rewind = 3)
    {
        for (int i = 0; i < 4 - rewind; i++)
        {
            transform.Rotate(Vector3.forward, 90);                                      //90�� ȸ��
            ChangeFrontDirection();                                                     //�ǸŹ��⺯��
            SwapWidthAndHeight();                                                       //��� ����,���� ũ�⺯��

            countRotation = (countRotation + 1) % 4;                                    //ȸ����++
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

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.instance.gameMode == GameManager.GameMode.Builder)
        {
            isMoving = true;    //�̵����� ��ȯ
            //�巡�׷� ���÷��� ���� ���� �����ϰ� �ִ� ��带 walkable�� ����
            ChangeWalkebleOfOccupiedNode(true);
        }
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

    Direction SetDirection() //�����߿� �ʱ�ȭ�� ���� ���� ���� ����, 90,270������ ���̿� �ʺ� �ٲ�� ������
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

    protected enum Direction { Left, Down, Right, Up }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    public bool displayGridGizmos;

    int index = 1;                             //창고 고유번호
    Vector2 worldPosition;                     //창고 월드포인트
    int frontSize;                             //창고 판매위치 노드크기
    int width;                                 //창고 노드너비
    int height;                                //창고 노드높이
    Direction fontDir;                         //창고 입구 방향

    Node[,] nodeOccupiedByWarehouse;
    Vector2[] frontPosition;

    int maxInvenSize;
    Item[] inventory;

    float nodeRadius;                           //노드 반지름
    float nodeDiameter;                         //노드 지름

    void Awake()
    {
        frontSize = 4;
        height = 4;
        width = 4;
        worldPosition = transform.position;
        fontDir = Direction.Left;
        inventory = new Item[maxInvenSize];
    }

    void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        nodeOccupiedByWarehouse = SetNodeOccupiedByShelf(worldPosition, width, height);    //SetShelfFrontPosition(); 이전에 실행되야함,  순서중요!!!
        frontPosition = SetFrontPosition(nodeOccupiedByWarehouse);           //SetNodeOccupiedByShelf();가 먼저 실행되야함, nodeOccupiedByShelf값이 있어야 ShelfFrontPosition 계산 가능
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

    Vector2[] SetFrontPosition(Node[,] occupiedNodes)    //판매위치의 월드좌표(판매위치는 매대 앞)
    {
        Vector2[] position = new Vector2[frontSize];

        switch (fontDir)
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

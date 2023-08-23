using UnityEngine;
using System;
using Random = UnityEngine.Random;
using JetBrains.Annotations;
using System.Linq;

public class Shelf : MonoBehaviour
{
    public bool displayGridGizmos;

    int shelfIndex;                             //매대 고유번호
    Vector2 worldPosition;                      //중심점 월드포인트
    int shelfFrontSize;                         //매대 판매위치 노드크기
    int shelfWidth;                             //매대 노드너비
    SalesDir salesDir;                          //현재 매대의 판매방향

    Node[,] nodeOccupiedByShelf;                //매대가 차지하고 있는 노드들
    Vector2[] ShelfFrontPosition;               //판매위치 노드의 중심좌표
    

    float nodeRadius;                           //노드 반지름
    float nodeDiameter;                         //노드 지름
    Item[] ItemSlot;                            //매대 아이템 슬롯

    void Awake()
    {
        shelfFrontSize = 4;
        shelfWidth = 1;
        worldPosition = transform.position;
        salesDir = SalesDir.Left;
        ItemSlot = new Item[shelfFrontSize];
    }
    // Start is called before the first frame update
    void Start()
    {
        nodeRadius = Nodefinding.instance.GetNodeRadius();
        nodeDiameter = nodeRadius * 2;
        shelfIndex = ShelfManager.instance.RequestShelfIndex();
        nodeOccupiedByShelf = SetNodeOccupiedByShelf(worldPosition, shelfWidth, shelfFrontSize);
        ShelfFrontPosition = SetShelfFrontPosition();

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
        Debug.Log(((ItemSlot[0] != null) ? ItemSlot[0].amount : 0) + "  ,  " + ((ItemSlot[1] != null) ? ItemSlot[1].amount : 0) + "  ,  " + ((ItemSlot[2] != null) ? ItemSlot[2].amount : 0) + "  ,  " + ((ItemSlot[3] != null) ? ItemSlot[3].amount : 0));
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

    Vector2[] SetShelfFrontPosition()    //판매위치의 월드좌표(판매위치는 매대 앞)
    {
        Vector2[] position = new Vector2[shelfFrontSize];

        switch (salesDir)
        {
            case SalesDir.Left:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 왼쪽줄의 worldpoint - 노드지름 
                    position[i] = new Vector2(nodeOccupiedByShelf[shelfWidth - 1, i].worldPosition.x - nodeDiameter, nodeOccupiedByShelf[shelfWidth - 1, i].worldPosition.y);
                break;
            case SalesDir.Right:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 오른쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(nodeOccupiedByShelf[shelfWidth - 1, i].worldPosition.x + nodeDiameter, nodeOccupiedByShelf[shelfWidth - 1, i].worldPosition.y);
                break;
            case SalesDir.Up:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 위쪽줄의 worldpoint + 노드지름 
                    position[i] = new Vector2(nodeOccupiedByShelf[i, shelfWidth - 1].worldPosition.x, nodeOccupiedByShelf[i, shelfWidth - 1].worldPosition.y + nodeDiameter);
                break;
            case SalesDir.Down:
                for (int i = 0; i < shelfFrontSize; i++)    //가장 위쪽줄의 worldpoint - 노드지름
                    position[i] = new Vector2(nodeOccupiedByShelf[i, 0].worldPosition.x, nodeOccupiedByShelf[i, 0].worldPosition.y - nodeDiameter);
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

    enum Type { SmallShelf, MediumShelf, LargeShelf }
    enum SalesDir { Left, Right, Up, Down }

}

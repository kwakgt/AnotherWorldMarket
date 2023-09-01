using UnityEngine;

public class Nodefinding : MonoBehaviour
{
    public static Nodefinding instance;
    AstarGrid grid;

    void Awake()
    {
        grid = GetComponent<AstarGrid>();
        instance = this;
    }

    public Node RequestNode(Vector2 worldpoint)   //월드좌표를 노드좌표로 변경
    {
        return grid.NodeFromWorldPoint(worldpoint);
    }


    public Node[,] RequestNodeArea(Vector2[,] worldpoints, int sizeX, int sizeY)  //월드좌표 배열를 노드좌표 배열로 변경
    {
        Node[,] nodes = new Node[sizeX, sizeY];
        for (int y = 0; y < sizeY; y++)
        {
            for(int x = 0; x < sizeX; x++)
            {
                nodes[x, y] = grid.NodeFromWorldPoint(worldpoints[x, y]);
            }
        }

        return nodes;
    }

    public float GetNodeRadius()
    {
        return grid.nodeRadius;
    }

    public Vector2 GetGridWorldSize()
    {
        return grid.gridWorldSize;
    }

    public Vector2 GetGridWorldPosition()
    {
        return grid.transform.position;
    }
}

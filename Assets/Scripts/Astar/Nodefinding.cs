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

    public Node RequestNode(Vector2 worldpoint, int gridIndex)   //월드좌표를 노드좌표로 변경
    {
        return grid.NodeFromWorldPoint(worldpoint, gridIndex);
    }


    public Node[,] RequestNodeArea(Vector2[,] worldpoints, int sizeX, int sizeY, int gridIndex)  //월드좌표 배열를 노드좌표 배열로 변경
    {
        Node[,] nodes = new Node[sizeX, sizeY];
        for (int y = 0; y < sizeY; y++)
        {
            for(int x = 0; x < sizeX; x++)
            {
                nodes[x, y] = grid.NodeFromWorldPoint(worldpoints[x, y], gridIndex);
            }
        }

        return nodes;
    }

    public float GetNodeRadius()
    {
        return grid.nodeRadius;
    }

    public Vector2 GetGridWorldSize(int gridIndex)
    {
        return grid.grids[gridIndex].gridWorldSize;
    }

    public Vector2 GetGridCenterPosition(int gridIndex)
    {
        return grid.grids[gridIndex].centerPosition;
    }

    public int GetGridIndex(Vector2 target)
    {
        for (int i = 0; i < grid.grids.Count; i++)
        {
            if (target.x > grid.grids[i].centerPosition.x - grid.grids[i].gridSizeX && target.x < grid.grids[i].centerPosition.x + grid.grids[i].gridSizeX
                && target.y > grid.grids[i].centerPosition.y - grid.grids[i].gridSizeY && target.y < grid.grids[i].centerPosition.y + grid.grids[i].gridSizeY)
            {
                //TARGET이 해당 그리드 안에 있다면
                return grid.grids[i].gridIndex;
            }
        }
        return -1;
    }
}

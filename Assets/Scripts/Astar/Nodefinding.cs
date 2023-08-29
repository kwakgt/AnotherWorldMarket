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

    public Node RequestNode(Vector2 worldpoint)   //������ǥ�� �����ǥ�� ����
    {
        return grid.NodeFromWorldPoint(worldpoint);
    }


    public Node[,] RequestNodeArea(Vector2[,] worldpoints, int sizeX, int sizeY)  //������ǥ �迭�� �����ǥ �迭�� ����
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarGrid : MonoBehaviour
{
    public bool displayGridGizmos;

    public Vector2 gridWorldSize;
    int gridSizeX, gridSizeY;
    Node[,] grid;
    
    public LayerMask unwalkableMask;
    public float nodeRadius;
    float nodeDiameter;

    public TerrainType[] walkableRegions;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    LayerMask walkableMask;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value; //A |= B : A와 B를 or연산하여 A에 대입한다. 레이어는 2의 배수여서 or연산이 합연산과 비슷하다. 여러개의 레이어에 검사 하기 위해
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);  //레어이 번호, 패널티 가중치 저장
        }

        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask)); // if no collider2D is returned by overlap circle, then this node is walkable

                int movementPenalty = 10;    //노드 이동 기본 가중치

                Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPoint, nodeRadius, walkableMask);  //Terraine 충돌체 가져오기
                if (colliders.Length > 0)
                {
                    walkableRegionsDictionary.TryGetValue(colliders[0].gameObject.layer, out movementPenalty);  //Terraine은 노드당 1개만 있기때문에 첫번째 충돌체만 가져와서 가중치 설정
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);     //노드칸에 수집한 정보 입력
            }
        }


        //BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize)   //가중치 블러처리
    {
        int kernelSize = blurSize * 2 + 1;          //실제 커널사이즈, 중앙사각형을 포함한 홀수여야 함
        int kernelExtents = (kernelSize - 1) / 2;   //중앙사각형과 가장자리 사이의 사각형의 수

        //커널은 중앙사각형을 기준으로 kernelSize * kernelSize 크기의 맵이고, 결과적으로 중앙사각형에는 커널안에 있는 사각형들의 가중치값의 합이 들어간다.
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY]; //커널에서 먼저 가로사각형안의 합을 구하고, 
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];   //그다음 세로사각형의 합을 구하면, 커널안에 있는 모든 사각형의 합이 나온다. 

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents); //Mathf.Clamp(Value,Min,Max) : Value가 Min이하면 Min을, Max이상이면 Max를, Min과 Max 사이면 그대로 Value를 반환한다.
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;  //왼쪽 가장자리에 있는 사각형은 왼쪽에 커널사각형이 없기때문에 자기자신으로 채운다. x=0일때 사각형값 초기화
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                //현재 사각형을 구할려면 이전 사각형의 왼쪽 가장자리 사각형 가중치을 제거하고, 오른쪽 가장자리 사각형 가중치를 더한다.
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX); //제거할 왼쪽 가장자리 사각형 인덱스
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);    //더할 오른쪽 가장자리 사각형 인덱스

                //현재 사각형값 = 이전 사각형값 - 이전 사각형값의 왼쪽 가장자리의 가중치 값 + 이전 사각형값의 오른쪽 가장자리의 가중치 값
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY]; //가장 아래에 있는 사각형은 아래에 커널사각형이 없기때문에 자기자신으로 채운다. y=0일때 사각형값 초기화
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));  //사각형값(합한 패널티값)을 커널개수로 나눠 최종 평균 가중치값을 구한다.
            grid[x, 0].movementPenalty = blurredPenalty;                                                            //그리드 노드의 가중치 변수에 최종 가중치 값을 넣는다.

            for (int y = 1; y < gridSizeY; y++)
            {
                //이전에 가로값을 전부 구해서 세로값끼리 더하기만 하면 모든 커널 사각형의 합이 된다. 
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY); //가로계산할 때와 동일하다.
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                //현재 사각형값 = 이전 사각형값 - 이전 사각형값의 아래쪽 가장자리의 가중치 값 + 이전 사각형값의 위쪽 가장자리의 가중치 값
                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;    //최종 계산된 가중치 값을 그리드 노드의 가중치 변수에 넣는다.

                //최종 계산된 가중치 값들의 최대값과 최소값을 구한다.
                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }

    }

    public List<Node> GetNeighbours(Node node, int depth = 1) //동서남북 대각선에 인접한 노드를 반환하는 함수
    {
        List<Node> neighbours = new List<Node>();   //인접한 노드 목록

        for (int x = -depth; x <= depth; x++)
        {
            for (int y = -depth; y <= depth; y++)
            {
                if (x == 0 && y == 0)   continue;           //자신 제외
                //if (Mathf.Abs(x) == Mathf.Abs(y)) continue; //대각선 제외

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (!grid[node.gridX, checkY].walkable || !grid[checkX, node.gridY].walkable) continue; //코너를 돌 때 벽을 통과하지 않기위해 

                    if (Mathf.Abs(x) == Mathf.Abs(y))    //대각선으로 막힌곳을 뚫을수 없게 하기 위해
                    {
                        if ((grid[checkX, node.gridY].walkable || grid[node.gridX, checkY].walkable))   //대각선 노드와 인접한 두 노드가 장애물이면 대각선 이동을 할 수 없다.
                        {
                            neighbours.Add(grid[checkX, checkY]);
                        }
                    }
                    else
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector2 worldPosition)   //월드좌표를 노드좌표로 변경
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Node ClosestWalkableNode(Node node)  //가장 가까운 이동가능한 노드 찾기
    {
        int maxRadius = Mathf.Max(gridSizeX, gridSizeY) / 2;
        for (int i = 1; i < maxRadius; i++)
        {
            Node n = FindWalkableInRadius(node.gridX, node.gridY, i);
            if (n != null)
            {
                return n;
            }
        }
        return null;
    }
    Node FindWalkableInRadius(int centreX, int centreY, int radius) //radius거리 안에 이동가능한 노드 찾기
    {

        for (int i = -radius; i <= radius; i++)
        {
            int verticalSearchX = i + centreX;
            int horizontalSearchY = i + centreY;

            // top, ↖ ↑ ↗
            if (InBounds(verticalSearchX, centreY + radius))
            {
                if (grid[verticalSearchX, centreY + radius].walkable)
                {
                    return grid[verticalSearchX, centreY + radius];
                }
            }

            // bottom,  ↙ ↓ ↘
            if (InBounds(verticalSearchX, centreY - radius))
            {
                if (grid[verticalSearchX, centreY - radius].walkable)
                {
                    return grid[verticalSearchX, centreY - radius];
                }
            }
            // right,   ↘ → ↗
            if (InBounds(centreY + radius, horizontalSearchY))
            {
                if (grid[centreX + radius, horizontalSearchY].walkable)
                {
                    return grid[centreX + radius, horizontalSearchY];
                }
            }

            // left,    ↖ ← ↙
            if (InBounds(centreY - radius, horizontalSearchY))
            {
                if (grid[centreX - radius, horizontalSearchY].walkable)
                {
                    return grid[centreX - radius, horizontalSearchY];
                }
            }

        }

        return null;

    }

    bool InBounds(int x, int y) //x좌표와 y좌표가 그리드 안에 있는지 확인
    {
        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                //Lerp(a,b,t) : t는 0~1사이에 속해야하고, a와 b사이 값중에서 t의 보간비율만큼의 값을 반환. ex) Lerp(0,10,0.5f) = 5
                //InverseLerp(a,b,value) : Lerp와 반대로 a와 b 사이의 value값을 통해 t(보간)값을 반환. a와 b를 넘은 값은 각각 0과 1을 반환. ex) InverseLerp(0,10,5) = 0.5f, InverseLerp(0,10,12) = 1f;
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .5f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}

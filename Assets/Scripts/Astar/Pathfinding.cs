using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    static Pathfinding instance;
    AstarGrid grid;

    void Awake()
    {
        grid = GetComponent<AstarGrid>();
        instance = this;
    }

    public static Vector2[] RequestPath(Vector2 from, Vector2 to)
    {
        return instance.FindPath(from, to);
    }

    Vector2[] FindPath(Vector2 from, Vector2 to)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(from);
        Node targetNode = grid.NodeFromWorldPoint(to);
        startNode.parent = startNode;

        if (!startNode.walkable)                                //스타트 노드가 이동불가 노드이면
        {
            startNode = grid.ClosestWalkableNode(startNode);    //가장 가까운 이동가능 노드 찾기
        }
        if (!targetNode.walkable)                               //타겟 노드가 이동불가 노드이면
        {
            targetNode = grid.ClosestWalkableNode(targetNode);  //가장 가까운 이동가능 노드 찾기
        }

        if (startNode.walkable && targetNode.walkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);  //열린목록
            HashSet<Node> closedSet = new HashSet<Node>();      //닫힌목록,HashSet은 해시(Hash)를 기반으로 값을 관리하므로 인덱스를 사용하여 값을 가져올 수 없음,중복된 값이 없음
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();   //현재노드에 첫번째 노드 저장하고 열린목록에서 제거, 처음에는 시작노드
                closedSet.Add(currentNode);                 //현재노드를 닫힌목록에 추가

                if (currentNode == targetNode)  //현재노드가 타겟노드와 같으면 경로찾기 완료
                {
                    sw.Stop();
                    //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))     //인접노드 순회
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))   //인접노드가 장애물이거나 닫힌목록에 포함되어 있으면 건너뛰기
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistanceCost(currentNode, neighbour) + TurningCost(currentNode, neighbour); //인접노드 G비용 계산
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))   //계산된 인접노드 G비용이 이전 G비용보다 작으면
                    {
                        neighbour.gCost = newMovementCostToNeighbour;                                   //인접노드 G비용을 새G비용으로 변경
                        neighbour.hCost = GetDistanceCost(neighbour, targetNode);                       //H비용 계산
                        neighbour.parent = currentNode;                                                 //인접노드 부모 현재노드로 변경

                        if (!openSet.Contains(neighbour))                                               //열린목록에 인접노드가 없으면 열린목록에 추가
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);                                              //인접노드가 열린목록에 있으면 열린목록 Heap 재정렬
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);     //타겟노드로부터 부모노드를 이용하여 시작노드까지 경로 추적
        }

        return waypoints;

    }


    int TurningCost(Node from, Node to)
    {
        /*
		Vector2 dirOld = new Vector2(from.gridX - from.parent.gridX, from.gridY - from.parent.gridY);
		Vector2 dirNew = new Vector2(to.gridX - from.gridX, to.gridY - from.gridY);
		if (dirNew == dirOld)
			return 0;
		else if (dirOld.x != 0 && dirOld.y != 0 && dirNew.x != 0 && dirNew.y != 0) {
			return 5;
		}
		else {
			return 10;
		}
		*/

        return 0;
    }

    Vector2[] RetracePath(Node startNode, Node endNode) //도착노드에서 부모노드를 통한 경로 역추적
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)        //현재노드가 시작노드가 아니라면
        {
            path.Add(currentNode);              //현재노드를 path에 추가
            currentNode = currentNode.parent;   //현재노드에 부모노드 넣기
        }
        path.Add(startNode);                    //스타트 노드 넣기
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);   //역추적이므로 순서를 바꿔서 올바른 경로로 변경
        return waypoints;

    }

    Vector2[] SimplifyPath(List<Node> path)             //리스트를 배열로 변경
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)               //방향이 같으면 생략하고 다르면 waypoints에 저장
            {
                waypoints.Add(path[i - 1].worldPosition);   //도착노드 → 시작노드이므로 path[i]: 이전노드, path[i - 1]: 현재노드, path[i - 2]: 다음노드. path[i - 1]에서 방향이 꺾이므로 waypoint가 됨.
            }
            directionOld = directionNew;
        }
        waypoints.Add(path[path.Count - 1].worldPosition);  //스타트 노드 넣기
        return waypoints.ToArray(); //waypoints의 값들을 새 배열에 복사합니다.
    }

    int GetDistanceCost(Node nodeA, Node nodeB) //가로이동 10, 대각선이동 14
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  //dstY만큼 대각선이동, dstX - dstY만큼 가로이동
        return 14 * dstX + 10 * (dstY - dstX);      //dstX만큼 대각선이동, dstY - dstX만큼 세로이동
    }


}
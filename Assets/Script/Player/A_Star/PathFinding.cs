using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    public GRID grid;
    private List<Node> Path;

    //public Transform seeker, target;
    
    void Awake()
    {
        grid = GetComponent<GRID>();
        
    }


    public List<Vector3> Return_Path(Transform player)
    {
        List<Vector3> route = new List<Vector3>();
        
        for(int i = 0; i < Path.Count; i++)
        {
            route.Add(new Vector3(Path[i].worldPosition.x,player.position.y,Path[i].worldPosition.y));
        }

        return route;
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWolrdPoint(startPos);
        Node targetNode = grid.NodeFromWolrdPoint(targetPos);//시작점과 목표지점 초기화

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);//경로 노드 리스트
        HashSet<Node> closedSet = new HashSet<Node>();//이미 탐색을 마친 노드 리스트
        openSet.Add(startNode);//시작점

        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if(currentNode == targetNode)//타겟 노드를 발견하면 탈출
            {
                sw.Stop();
                //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode,targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;
                
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour,targetNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }


        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        grid.path = path;
        Path = path;
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}

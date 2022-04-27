using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour
{
    public GameObject line;
    LineRenderer lineRander;

    Grid grid;
    public Node start, end;
    public bool finding;

    private void Awake()
    {
        lineRander = line.GetComponent<LineRenderer>();
        grid = GetComponent<Grid>();
        StartGrid();
    }

    public void StartGrid()
    {
        StopCoroutine("FindPath");
        line.SetActive(false);
        finding = false;

        bool success = grid.CreateGrid();

        if(success)
        {
            start = grid.StartNode;
            end = grid.EndNode;
            start.ChangeStart = true;
            end.ChangeEnd = true;
        }
    }

    public void StartFinding(bool search)
    {
        StopCoroutine("FindPath");
        line.SetActive(false);
        finding = false;
        grid.ResetGrid();
        if(search) StartCoroutine("FindPath");
    }

    IEnumerator FindPath()
    {
        finding = true;
        bool pathSuccess = false;        

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i<openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == end)
            {
                pathSuccess = true;
                break;
            }

            yield return new WaitUntil(() => finding);
            //yield return new WaitForSeconds(0.1f);

            if (currentNode != start)
                currentNode.ChangeColor = Color.Lerp(Color.cyan, Color.white, 0.2f);

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable  || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, end);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        if (neighbour.walkable && !neighbour.end)
                            neighbour.ChangeColor = Color.Lerp(Color.green, Color.white, 0.2f);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            DrawingLine(RetracePath(start, end));
        }
        finding = false;
    }
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].ground.transform.position + Vector3.up * 0.1f);
            }
            directionOld = directionNew;
        }
        waypoints.Add(start.ground.transform.position + Vector3.up * 0.1f);
        return waypoints.ToArray();
    }

    public void DrawingLine(Vector3[] waypoints)
    {
        line.SetActive(true);
        lineRander.positionCount = waypoints.Length;
        for (int i = 0; i < waypoints.Length; i++)
        {
            lineRander.SetPosition(i, waypoints[i]);
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

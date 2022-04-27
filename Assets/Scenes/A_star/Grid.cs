using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject groundPrefab;
    GameObject parentGrid;

    public Vector2 gridWorldSize;

    Node[,] grid;

    private void Awake()
    {
    }

    public bool CreateGrid()
    {
        if (gridWorldSize.x < 2 || gridWorldSize.x > 101 || gridWorldSize.y < 2 || gridWorldSize.y > 51)
            return false;

        float cameraY = gridWorldSize.x * 0.42f > gridWorldSize.y * 0.87f ? gridWorldSize.x * 0.42f : gridWorldSize.y * 0.87f;
        transform.position = new Vector3(0, cameraY, 0);

        if (parentGrid != null)
            Destroy(parentGrid);
        parentGrid = new GameObject("parentGrid");

        grid = new Node[(int)gridWorldSize.x, (int)gridWorldSize.y];
        Vector3 worldBottomLeft = Vector3.zero - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < (int)gridWorldSize.x; x++)
        {
            for (int y = 0; y < (int)gridWorldSize.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x + 0.5f) + Vector3.forward * (y + 0.5f);
                GameObject obj = Instantiate(groundPrefab, worldPoint, Quaternion.Euler(90, 0, 0));
                obj.transform.parent = parentGrid.transform;
                grid[x, y] = new Node(obj, true, x, y);
            }
        }
        return true;
    }

    public void ResetGrid()
    {
        for (int x = 0; x < (int)gridWorldSize.x; x++)
        {
            for (int y = 0; y < (int)gridWorldSize.y; y++)
            {
                if (grid[x, y].walkable && !grid[x, y].start && !grid[x,y].end)
                {
                    grid[x, y].ChangeNode = true;
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int[,] temp = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
        bool[] walkableUDLR = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            int checkX = node.gridX + temp[i, 0];
            int checkY = node.gridY + temp[i, 1];
            if (checkX >= 0 && checkX < (int)gridWorldSize.x && checkY >= 0 && checkY < (int)gridWorldSize.y)
            {
                if (grid[checkX, checkY].walkable)
                    walkableUDLR[i] = true;
                neighbours.Add(grid[checkX, checkY]);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (walkableUDLR[i] || walkableUDLR[(i + 1) % 4])
            {
                int checkX = node.gridX + temp[i, 0] + temp[(i + 1) % 4, 0];
                int checkY = node.gridY + temp[i, 1] + temp[(i + 1) % 4, 1];
                if (checkX >= 0 && checkX < (int)gridWorldSize.x && checkY >= 0 && checkY < (int)gridWorldSize.y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodePoint(Vector3 rayPosition)
    {
        int x = (int)(rayPosition.x + gridWorldSize.x / 2);
        int y = (int)(rayPosition.z + gridWorldSize.y / 2);

        return grid[x, y];
    }

    public Node StartNode
    {
        get
        {
            grid[0, 0].start = true;
            return grid[0, 0];
        }
    }
    public Node EndNode
    {
        get
        {
            grid[(int)gridWorldSize.x - 1, (int)gridWorldSize.y - 1].end = true;
            return grid[(int)gridWorldSize.x - 1, (int)gridWorldSize.y - 1];
        }
    }
}

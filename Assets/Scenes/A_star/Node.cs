using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject ground;
    public bool walkable;
    public int gridX;
    public int gridY;

    public bool start;
    public bool end;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(GameObject _ground, bool _walkable, int _gridX, int _gridY)
    {
        ground = _ground;
        walkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public bool ChangeNode
    {
        set
        {
            Color color = value ? Color.white : Color.gray;
            walkable = value;
            ChangeColor = color;                       
        }
    }
    public int fCost
    {
        get{ return gCost + hCost; }
    }

    public bool ChangeStart
    {
        set
        {
            if (value)
            {
                start = value;
                ChangeColor = Color.Lerp(Color.blue, Color.white, 0.2f);
            }
            else
            {
                start = value;
                ChangeNode = walkable;
            }
        }
    }
    public bool ChangeEnd
    {
        set
        {
            if (value)
            {
                end = value;
                ChangeColor = Color.Lerp(Color.red, Color.white, 0.2f);
            }
            else
            {
                end = value;
                ChangeNode = walkable;
            }
        }
    }
    public Color ChangeColor 
    {
        set{ground.GetComponent<MeshRenderer>().material.color = value;}
    }
}
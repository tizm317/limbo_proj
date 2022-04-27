using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{ 
    Grid grid;
    Main main;

    int settingButton = 1;

    bool pauseBut;

    private void Awake()
    {
        main = GetComponent<Main>();
        grid = GetComponent<Grid>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) )
        {
            Node node = RayCast();
            if (node != null)
            {
                if(node.start || node.end)
                    StartCoroutine("SwitchStartEnd", node);
                else
                    StartCoroutine("ChangeWalkable", node);
            }
        }
    }

    IEnumerator SwitchStartEnd(Node node)
    {
        bool start = node.start;
        Node nodeOld = node;
        while (Input.GetMouseButton(0))
        {
            node = RayCast();
            if (node != null && node != nodeOld)
            {
                if (start && !node.end)
                {
                    node.ChangeStart = true;
                    main.start = node;
                    nodeOld.ChangeStart = false;
                    nodeOld = node;
                }
                else if (!start && !node.start)
                {
                    node.ChangeEnd = true;
                    main.end = node;
                    nodeOld.ChangeEnd = false;
                    nodeOld = node;
                }
            }
            yield return null;
        }
    }

    IEnumerator ChangeWalkable(Node node)
    {
        bool walkable = !node.walkable;

        while (Input.GetMouseButton(0))
        {
            node = RayCast();
            if (node != null && !node.start && !node.end) 
            {
                node.ChangeNode = walkable;
            }
            yield return null;
        }
    }

    public Node RayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject obj = hit.collider.gameObject;
            return grid.NodePoint(obj.transform.position);
        }
        return null;
    }
    void ReconstructionGrid(int windowId)
    {
        string value;

        GUIStyle textStyle = new GUIStyle("TextField");
        GUILayout.BeginVertical("box");
        GUILayout.Label("Size X : 2 ~ 100");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Size X", GUILayout.MinWidth(50));
        value = GUILayout.TextField(grid.gridWorldSize.x.ToString(), textStyle, GUILayout.MinWidth(50));
        grid.gridWorldSize.x = int.Parse(value);
        GUILayout.EndHorizontal();
        GUILayout.Label("Size Y : 2 ~ 50");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Size Y", GUILayout.MinWidth(50));
        value = GUILayout.TextField(grid.gridWorldSize.y.ToString(), textStyle, GUILayout.MinWidth(50));
        grid.gridWorldSize.y = int.Parse(value);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        if (GUILayout.Button("Change Grid"))
        {
            main.StartGrid();
            pauseBut = false;
        }
        if (!main.finding && pauseBut)
        {
            if (GUILayout.Button("Resume Search"))
            {
                main.finding = true;
                pauseBut = false;
            }            
        }
        else
        {
            if (GUILayout.Button("Start Search"))
            {
                main.StartFinding(true);                
            }            
        }
        if (pauseBut)
        {
            if (GUILayout.Button("Cancel Search"))
            {
                pauseBut = false;
                main.StartFinding(false);
            }
        }
        else
        {
            if (GUILayout.Button("Pause Search"))
            {
                if (main.finding)
                {
                    pauseBut = true;
                    main.finding = false;
                }                
            }
        }

    }
    private void OnGUI()
    {

        if (GUILayout.Button("Menu"))
        {
            settingButton *= -1;
        }
        if (settingButton == 1)
        {
            GUILayout.Window(0, new Rect(10, 30, 2, 2), ReconstructionGrid, "Settings");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUILine : MonoBehaviour
{
    // UI 용 Line Renderer
    // 포지션 배열

    public GameObject UIline;
    List<Vector3> posList = new List<Vector3>();
    List<GameObject> lineList = new List<GameObject>();
    int previousPathCount = 0;
    [SerializeField]
    private int count = 0;

    void Start()
    {
        Init();
    }

    void Init()
    {
        UIline = Managers.Resource.Instantiate("UI/Scene/UILine");
        UIline.transform.SetParent(transform);
        UIline.transform.localPosition = new Vector3(0, 0, 0);
        //posList.Add(UIline.transform.position);
    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        Vector3 middle = (end + start) /2 ;
        UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        UIline.transform.localPosition = middle;
        Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,  Vector3.Distance(start, end));

    }

    public void DrawLine(Vector3 start, List<Vector3> path, bool isObstacle)
    {
        //count = path.Count;

        if (!isObstacle)
        {
            return;
        }
            

        // ~ing
        if (path != null)
        {

            for (int i = 0; i < path.Count - previousPathCount - 1; i++) 
            {
                //temp.transform.SetParent(transform);

                // path 리스트가 유지되나??? 뭐지 

                lineList.Add(Managers.Resource.Instantiate("UI/Scene/UILine", transform));

                Vector3 middle;
                if (i == 0)
                {
                    middle = (path[0] + start) / 2;
                    lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[0] - start);
                    Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(start, path[0]));

                }
                else
                {
                    middle = (path[i] + path[i + 1]) / 2;
                    lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[i+1] - path[i]);
                    Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(path[i], path[i + 1]));
                }
                lineList[i].transform.localPosition = middle;
            }

        }

        previousPathCount = path.Count;

        //Vector3 middle = (end + start) / 2;
       
        //UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        //UIline.transform.localPosition = middle;
        //Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        //UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(start, end));

    }
    public void ClearLine()
    {
        foreach(GameObject n in lineList)
        {
            Managers.Resource.Destroy(n);
        }
        lineList.Clear();
    }
}

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
        UIline.transform.localPosition = transform.GetChild(0).position;
        UIline.SetActive(false);
        //posList.Add(UIline.transform.position);

    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        UIline.SetActive(true);
        Vector3 middle = (end + start) /2 ;
        UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        UIline.transform.localPosition = middle;
        Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,  Vector3.Distance(start, end));

    }

    public void DrawLine(Vector3 start, List<Vector3> path, bool isObstacle)
    {
        //count = path.Count;
        UIline.SetActive(true);

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
                    path[0] = new Vector3(path[0].x,path[0].z,0);

                    middle = (path[0] + start) / 2;
                    lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[0] - start);
                    Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(start, path[0]));

                }
                else
                {
                    //path[i] = new Vector3(path[i].x, path[i].z, 0);
                    //path[i+1] = new Vector3(path[i+1].x, path[i+1].z, 0);

                    //middle = (path[i] + path[i + 1]) / 2;
                    middle = (new Vector3(path[i].x, path[i].z, 0) + new Vector3(path[i + 1].x, path[i + 1].z, 0)) / 2;
                    lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[i+1] - path[i]);
                    Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(path[i], path[i + 1]));
                }
                lineList[i].transform.localPosition = middle;
            }

        }

        //previousPathCount = path.Count;

        //Vector3 middle = (end + start) / 2;

        //UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        //UIline.transform.localPosition = middle;
        //Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        //UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(start, end));

    }
    public void ClearLine()
    {
        UIline.SetActive(false);
        foreach (GameObject n in lineList)
        {
            Managers.Resource.Destroy(n);
        }
        lineList.Clear();
    }
}

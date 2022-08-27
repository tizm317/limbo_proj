using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUILine : MonoBehaviour
{
    // UI 용 Line Renderer
    // 포지션 배열

    // UIline 변수는 직선 경로용
    // lineList 리스트는 장애물 있어서 꺾이는 경로용

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
        UIline.transform.SetAsFirstSibling();

    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        // 장애물 없는 직선 라인

        UIline.SetActive(true);
        Vector3 middle = (end + start) /2 ;
        UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        UIline.transform.localPosition = middle;
        Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,  Vector3.Distance(start, end));

    }


    public void DrawLine(Vector3 start, List<Vector3> path, bool isObstacle)
    {
        // 장애물 있는 경우

        //count = path.Count;
        //UIline.SetActive(true);

        if (!isObstacle)
        {
            return;
        }
            

        if (path != null)
        {
            lineList.Clear();

            for (int i = 0; i < path.Count - previousPathCount - 1; i++) 
            {

                lineList.Add(Managers.Resource.Instantiate("UI/Scene/UILine", transform));
                Vector3 middle;

                if (i == 0)
                {
                    // 맨처음 위치 때문에 따로(start)
                    // 위치 조절
                    path[0] = new Vector3(path[0].x,path[0].z,0);
                    middle = (path[0] + start) / 2;
                    lineList[i].transform.localPosition = middle;
                    lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[0] - start);
                    // 크기 조절
                    //Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    //lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, Vector3.Distance(start, path[0]));
                    lineList[i].GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
                    //float dist = Vector3.Distance(start, path[0]);
                    //lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, dist);
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, 3.0f);
                }
                else
                {
                    // 위치 조절
                    Vector3 curPos = new Vector3(path[i].x, path[i].z, 0);
                    Vector3 nextPos = new Vector3(path[i + 1].x, path[i + 1].z, 0);
                    //Vector3 initialPos = new Vector3(-1,-1,-1);

                    middle = (nextPos + curPos) / 2;
                    //float dist = Vector3.Distance(curPos, nextPos);
                    lineList[i].transform.localPosition = middle;

                    // 방향 조절
                    float angle;
                    angle = Mathf.Atan2(nextPos.y - curPos.y, nextPos.x - curPos.x) * Mathf.Rad2Deg;
                    lineList[i].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                    
                   
                    // 크기 조절
                    //Vector2 size = lineList[i].GetComponent<RectTransform>().sizeDelta;
                    lineList[i].GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
                    //lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, dist);
                    lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, 3.0f);
                }

                // child 위치 조정 (라인이 플레이어,도착 마크 가려서)
                lineList[i].transform.SetAsFirstSibling();
            }

        }

    }
    public void ClearLine()
    {
        // 직선 라인 끄기
        UIline.SetActive(false);

        // 경로 리스트 지우기
        foreach (GameObject n in lineList)
        {
            Managers.Resource.Destroy(n);
        }
        lineList.Clear();
    }
}

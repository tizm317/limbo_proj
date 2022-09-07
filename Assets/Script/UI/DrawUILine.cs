using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUILine : MonoBehaviour
{
    // UI 용 Line Renderer
    // 포지션 배열

    // 1) UIline 변수는 직선 경로용
    public GameObject UIline;
    // 2) lineList 리스트는 장애물 있어서 꺾이는 경로용
    List<GameObject> lineList = new List<GameObject>();
    [SerializeField]
    private int count = 0; // 유니티에서 확인용

    void Start()
    {
        Init();
    }

    void Init()
    {
        // 직선용 UIline
        UIline = Managers.Resource.Instantiate("UI/Scene/UILine");
        UIline.transform.SetParent(transform);
        UIline.transform.localPosition = transform.GetChild(0).position;
        UIline.SetActive(false);

        UIline.transform.SetAsFirstSibling(); // hierachy 순서 지정
    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        // 장애물 없는 직선 라인
        UIline.SetActive(true);

        // 위치
        Vector3 middle = (end + start) /2 ;
        // 방향
        UIline.transform.rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        UIline.transform.localPosition = middle;
        // 크기
        Vector2 size = UIline.GetComponent<RectTransform>().sizeDelta;
        UIline.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,  Vector3.Distance(start, end));
        // 개수
        count = 1;
    }
    public void DrawLine(Vector3 start, List<Vector3> path, bool isObstacle)
    {
        // 장애물 있는 커브드 라인
        if (!isObstacle)
            return;
        if (path == null)
            return;

        lineList.Clear();

        for (int i = 0; i < path.Count - 1; i++)
        {
            lineList.Add(Managers.Resource.Instantiate("UI/Scene/UILine", transform));
            Vector3 middle;

            if (i == 0)
            {
                // 맨처음 위치 때문에 따로(start)
                // 위치
                path[0] = new Vector3(path[0].x, path[0].z, 0);
                middle = (path[0] + start) / 2;
                lineList[i].transform.localPosition = middle;
                // 방향
                float angle;
                Vector2 dir = (path[0] - start).normalized;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                lineList[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                //lineList[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, path[0] - start);
                // 크기
                lineList[i].GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
                //lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, 3.0f);
                float lineWidth = Vector2.Distance(start, path[0]);
                lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth, 3.0f);
            }
            else
            {
                // 위치
                Vector3 curPos = new Vector3(path[i].x, path[i].z, 0);
                Vector3 nextPos = new Vector3(path[i + 1].x, path[i + 1].z, 0);
                middle = (nextPos + curPos) / 2;
                lineList[i].transform.localPosition = middle;

                // 미니맵에서 2d 방향
                float angle;
                Vector2 dir = (nextPos - curPos).normalized;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //angle = Mathf.Atan2(nextPos.y - curPos.y, nextPos.x - curPos.x) * Mathf.Rad2Deg;
                //lineList[i].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                lineList[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);

                // 크기
                float lineWidth = Vector2.Distance(curPos, nextPos);
                lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(lineWidth, 3.0f);
                //lineList[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineWidth);
                lineList[i].GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
                //lineList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(3.0f, 3.0f);
            }

            // child 위치 조정 (라인이 플레이어,도착 마크 가려서)
            lineList[i].transform.SetAsFirstSibling();
        }
        // 개수
        count = lineList.Count;
    }
    public void ClearLine()
    {
        // 직선 라인 끄기
        UIline.SetActive(false);

        // 경로 리스트 내 lines 삭제
        foreach (GameObject n in lineList)
            Managers.Resource.Destroy(n);

        // 경로 리스트 지우기
        lineList.Clear();
    }
}

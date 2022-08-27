using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MiniMap : UI_Popup
{
    // 미니맵
    /* 
     미니맵
    미리 입력된, 맵 + 고정된 사물, npc 등은 정보로 받고
    나머지 움직이는 것들 (static 아닌것들)

    미니맵 범위 설정
    범위 외 정보 필요 x

    실시간 정보 : 플레이어 외 이동하는 것들
    나중에 서버에서 다른 플레이어 정보 받아와야 할듯?

    클릭해서 크기 조절 : 3번 누르면 다시 작아지게

     */

    enum GameObjects
    {
        Mask,
        MapImage,
        PlayerImage,
        DestinationImage,
        BorderImage,
        ZoomIn,
        ZoomOut,
        
    }
    enum Buttons
    {
        ZoomInButton,
        ZoomOutButton,
    }

    enum size
    {
        DefaultSize,
        MiddleSize,
        MaxSize,
        Off,
    }
    enum zoom
    {
        DefaultZoom,
        MiddleZoom,
        MaxZoom,
    }
    size curSize = size.DefaultSize;
    zoom curZoom = zoom.DefaultZoom;

    [SerializeField] Transform player;
    Player_Controller player_Controller;
    private GameObject Scene;
    private RectTransform playerImage;
    private RectTransform destinationImage;
    private RectTransform mapImage;
    private RectTransform boarderImage;
    private RectTransform mask;

    Dictionary<int, Data.Map> dict_map;

    // 길찾기
    PathFinding pathFinding;
    public GameObject linePrefab;
    Vector3 playerPos;
    Vector3 destination;
    DrawUILine drawUILine;

    float dist_PlayerDestinationImg; // 미니맵 마커 사이 거리
    const float tempDist = 100.0f;
    const float arriveDist = 1.0f;

    void Start()
    {
        Init();
    }

    void Update()
    {
        // 플레이어 위치(파란색), 목적지(빨간색) 표시
        playerPos = player.position;

        // 미니맵 플레이어, 도착지 이미지 방향
        RotationControl(playerImage);
        if (destinationImage)
            RotationControl(destinationImage);

        // 플레이어
        playerPos.y = player.position.z;
        playerPos.z = 0;

        switch (curZoom)
        {
            case zoom.DefaultZoom:
                playerImage.localPosition = playerPos;
                mapImage.localPosition = playerPos * -1;
                break;
            case zoom.MiddleZoom:
                 playerImage.localPosition = playerPos;
                mapImage.localPosition = playerPos * -2;
                break;
            case zoom.MaxZoom:
                playerImage.localPosition = playerPos;
                mapImage.localPosition = playerPos * -3;
                break;
        }

        // 플레이어 원점으로 두고 나머지 이동하도록 수정
        // 플레이어 반대방향으로 지도 반대로 이동(고정된 물체 한번에 같이 이동시키기 위해서)
        // mapImage.localPosition = playerPos * -1;
    }

    public override void Init()
    {
        base.Init();

        // 버튼 관련해서 추가
        //uI_InGame = GameObject.Find("UI_InGame").GetComponent<UI_InGame>();

        // 바인딩
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        //
        playerImage = GetObject((int)GameObjects.PlayerImage).GetComponent<RectTransform>();
        destinationImage = GetObject((int)GameObjects.DestinationImage).GetComponent<RectTransform>();
        mapImage = GetObject((int)GameObjects.MapImage).GetComponent<RectTransform>();
        boarderImage = GetObject((int)GameObjects.BorderImage).GetComponent<RectTransform>();
        mask = GetObject((int)GameObjects.Mask).GetComponent<RectTransform>();

        // zoom 관련 버튼 이벤트랑 묶기
        GetButton((int)Buttons.ZoomInButton).gameObject.BindEvent(OnZoomInButtonClicked);
        GetButton((int)Buttons.ZoomOutButton).gameObject.BindEvent(OnZoomOutButtonClicked);

        player = GameObject.Find("Player").transform;
        Scene = GameObject.Find("@Scene");
        player_Controller = Scene.GetComponent<Player_Controller>();

        // path 받아오려고
        pathFinding = GameObject.Find("A*").GetComponent<PathFinding>();

        Vector3 temp = new Vector3(0, 0, 0);

        destinationImage.gameObject.SetActive(false);

        BindEvent(mapImage.gameObject, (PointerEventData data) =>
        {
            if (data.pointerId != -1) return;
            //destinationImage.transform.localPosition = new Vector3(data.position.x - 784.375f, data.position.y - 316.25f, 0); // 이거 수정하자

            switch (curSize)
            {
                case size.DefaultSize:
                    destinationImage.transform.localPosition = new Vector3(data.position.x - playerImage.position.x, data.position.y - playerImage.position.y, 0);
                    break;
                case size.MiddleSize:
                    destinationImage.transform.localPosition = new Vector3(data.position.x - playerImage.position.x, data.position.y - playerImage.position.y, 0) / 2;
                    break;
                case size.MaxSize:
                    destinationImage.transform.localPosition = new Vector3(data.position.x  - playerImage.position.x, data.position.y - playerImage.position.y, 0) / 3;
                    break;
            }
            switch (curZoom)
            {
                case zoom.DefaultZoom:
                    break;
                case zoom.MiddleZoom:
                    destinationImage.transform.localPosition /= 2;
                    break;
                case zoom.MaxZoom:
                    destinationImage.transform.localPosition /= 3;
                    break;
            }

            // 수정 -> 덜덜거림
            temp.x = destinationImage.transform.localPosition.x + player.transform.position.x;
            temp.z = destinationImage.transform.localPosition.y + player.transform.position.z;
            temp.y = 1;

            player_Controller.Set_Destination(temp);
        }, Define.UIEvent.Click);


        #region 맵 마커
        // 지우기
        foreach (Transform child in mapImage.transform)
        {
            if(child.GetComponent<UI_Minimap_ObjImg>() == true)
                Managers.Resource.Destroy(child.gameObject);
        }

        // 맵 데이터
        // static object data
        dict_map = Managers.Data.MapDict;
        
        // 맵 오브젝트 개수만큼 붙이기
        for (int i = 0; i < dict_map.Count; i++) // 실제 정보 참고해서 몇개 만들지 고려
        {
            // 1. UI_Minimap_ObjImag 생성해서 MapImage 산하에 붙임
            GameObject item = Managers.UI.MakeSubItem<UI_Minimap_ObjImg>(parent: mapImage.transform).gameObject;
            UI_Minimap_ObjImg objImg = item.GetOrAddComponent<UI_Minimap_ObjImg>();
            // 2. 위치 설정
            objImg.setMarkerPos(i);
        }
        #endregion
    }

    public override bool IsPeek()
    {
        return Managers.UI.IsPeek(this);
    }
    public void SizeControl()
    {
        // 미니맵 사이즈 조절

        //if (!IsPeek())
        //    return;

        curSize = (size)(((int)curSize + 1) % 4);

        switch ((int)curSize)
        {
            case 0: 
                Zoom();
                //mapImage.localScale = new Vector3(1, 1, 0);
                mask.localScale = new Vector3(1, 1, 0);
                boarderImage.localScale = new Vector3(1, 1, 0);
                break;
            case 1:
                mask.localScale = new Vector3(2, 2, 0);
                boarderImage.localScale = new Vector3(2, 2, 0);
                //mapImage.localScale = new Vector3(2, 2, 0);
                break;
            case 2:
                mask.localScale = new Vector3(3, 3, 0);
                boarderImage.localScale = new Vector3(3, 3, 0);
                //mapImage.localScale = new Vector3(3, 3, 0);
                break;
            case 3:
                //Managers.UI.ClosePopupUI();
                this.gameObject.SetActive(false);
                break;
        }

    }
    public void Zoom(bool reverse = false)
    {
        // 미니맵 줌 기능
        //if (!IsPeek())
        //    return;

        if(reverse)
            curZoom = (zoom)(((int)curZoom + 2) % 3);
        else
            curZoom = (zoom)(((int)curZoom + 1) % 3);

        switch ((int)curZoom)
        {
            case 0:  // default
                mapImage.localScale = new Vector3(1, 1, 0);
                break;
            case 1:  // second
                mapImage.localScale = new Vector3(2, 2, 0);
                break;
            case 2: // max
                mapImage.localScale = new Vector3(3, 3, 0);
                break;
        }
    }

    // +, - 버튼으로 zoom 조절
    public void OnZoomInButtonClicked(PointerEventData data)
    {
        Zoom();
    }
    public void OnZoomOutButtonClicked(PointerEventData data)
    {
        Zoom(reverse:true);
    }

    void RotationControl(RectTransform image)
    {
        // 회전
        Vector3 compassRotation = image.transform.eulerAngles;
        compassRotation.z = player.eulerAngles.y;
        image.transform.eulerAngles = compassRotation * -1;
    }

    public void drawLine()
    {
        drawUILine = mapImage.GetComponent<DrawUILine>();

        drawUILine.ClearLine();
        List<Vector3> path = pathFinding.Return_Path(player);

        if (destinationImage.localPosition != null && player_Controller.get_isObstacle() == false)
        {
            // 장애물 없는 경우 - 직선코스
            // 코루틴
            StartCoroutine(Co_Draw_Line());
            return;

            //drawDestinationMark();
            //drawUILine.DrawLine(playerImage.localPosition, destinationImage.localPosition);
        }

        if (path.Count != 0)
        {
            // 장애물 있는 경우 - 커브드 코스
            // 직선 경로 코루틴 정지
            StopAllCoroutines();
            
            // 
            drawDestinationMark();
            drawUILine.DrawLine(playerImage.localPosition, path, player_Controller.get_isObstacle());
            return;
        }

        if (path.Count != 0 && Vector3.Distance(playerImage.position, destinationImage.position) < arriveDist)
        {
            // path 는 있으나, 거리가 가까운 경우(?)
            drawUILine.ClearLine();
            path.Clear();
        }
    }

    public void clearLine()
    {
        destinationImage.gameObject.SetActive(false);
        drawUILine.ClearLine();
    }

    public void drawDestinationMark()
    {
        // 목적지 빨간색 표시 그리기

        // 목적지 위치 받아옴
        destination = player_Controller.Get_Destination();

        if (destination.y >= player_Controller.magicNumber)
            return;

        dist_PlayerDestinationImg = Mathf.Abs(playerPos.magnitude - destination.magnitude);

        // 일정 거리(arriveDist) 이내면 목적지 표시 x
        if (dist_PlayerDestinationImg < arriveDist)
        {
            destinationImage.gameObject.SetActive(false);
            return;
        }

        // 목적지 미니맵 2d 위치
        destination.y = destination.z;
        destination.z = 0;
        destinationImage.localPosition = destination;

        destinationImage.gameObject.SetActive(true);
    }

    IEnumerator Co_Draw_Line()
    {
        dist_PlayerDestinationImg = tempDist; // while문 실행하기 위한 임시값 초기화
        
        while (dist_PlayerDestinationImg > arriveDist)
        {
            drawDestinationMark();
            drawUILine.DrawLine(playerImage.localPosition, destinationImage.localPosition);
            yield return null;
        }
        //Debug.Log("코루틴 끝");
    }

}

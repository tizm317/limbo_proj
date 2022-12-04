using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_NewMiniMap : UI_Base
{
    // MiniMap New Version
    #region 코루틴 Wrapper 메소드
    // predicate 조건 불충족하면, 대기함
    private void ProcessLater(Func<bool> predicate, Action job)
    {
        StartCoroutine(PorcessLaterRoutine());

        // Local
        IEnumerator PorcessLaterRoutine()
        {
            yield return new WaitUntil(predicate);
            job?.Invoke();
        }
    }
    #endregion
    enum GameObjects
    {
        Mask,
        MapImage,
        PlayerMarker,
        DestinationMarker,
        BorderImage1,
        BorderImage2,
        BossAliveMarker,
        BossDeadMarker,
        //UI_NpcMarker,
        //ZoomIn,
        //ZoomOut,
    }
    enum Buttons
    {
        ZoomInButton,
        ZoomOutButton,
    }
    enum Texts
    {
        MapNameText,
    }

    void Start()
    {
        Init();
    }

    Image[] images;
    RawImage mapImg;
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);
        if (_rrList.Count == 0) return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    [SerializeField] Transform player;
    [SerializeField] Transform BossTr;

    // 길찾기
    PathFinding pathFinding;
    public GameObject linePrefab;
    Vector3 playerPos;
    Vector3 destination;
    Vector3 BossPos;
    DrawUILine drawUILine;

    float dist_PlayerDestinationImg; // 미니맵 마커 사이 거리
    const float tempDist = 100.0f;
    const float arriveDist = 1.0f;

    Npc[] Npcs;
    List<Npc> patrolNpc = new List<Npc>();
    List<UI_NpcMarker> patrolNpcMarkers = new List<UI_NpcMarker>();


    private void Update()
    {
        // 플레이어 위치(파란색), 목적지(빨간색) 표시
        if (player == null) return;
        playerPos = player.position;

        // 보스몹
        // find boss enemy
        if (BossTr == null)
            findBoss();

        if (BossTr != null) // 맵에 보스 있는 경우
        {
            BossPos = BossTr.position;
            BossPos.y = BossPos.z;
            BossPos.z = 0;
            BossPos *= 6;
            
            if (BossTr.gameObject.GetComponent<EnemyStat>().Hp <= 0) // 죽은 경우
            {
                bossDeadImage.localPosition = BossPos;
                bossDeadImage.gameObject.SetActive(true);
                bossAliveImage.gameObject.SetActive(false);
                
                BossTr = null;
            }
            else
            {
                bossAliveImage.localPosition = BossPos;
                bossAliveImage.gameObject.SetActive(true);
                bossDeadImage.gameObject.SetActive(false);
            }
        }
        else // 맵에 보스 없는 경우
        {
            bossAliveImage.gameObject.SetActive(false);
            bossDeadImage.gameObject.SetActive(false);
        }


        // 미니맵 플레이어, 도착지 이미지 방향
        RotationControl(playerImage);
        if (destinationImage)
            RotationControl(destinationImage);

        // 플레이어
        playerPos.y = player.position.z;
        playerPos.z = 0;
        playerPos *= 6;

        for(int i = 0; i < patrolNpc.Count; i++)
        {
            Vector3 npcPos = patrolNpc[i].transform.position;
            npcPos.y = npcPos.z;
            npcPos.z = 0;
            npcPos *= 6;

            patrolNpcMarkers[i].gameObject.transform.localPosition = npcPos;
            patrolNpcMarkers[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        switch (curZoom)
        {
            case zoom.DefaultZoom:
                playerImage.localPosition = playerPos;
                if(BossTr != null)
                    bossAliveImage.localPosition = BossPos;
                mapImage.localPosition = playerPos * -1; // 지도가 움직이게
                break;
            case zoom.MiddleZoom:
                playerImage.localPosition = playerPos;
                if (BossTr != null)
                    bossAliveImage.localPosition = BossPos;
                mapImage.localPosition = playerPos * -1.3f;
                break;
            case zoom.MaxZoom:
                playerImage.localPosition = playerPos;
                if (BossTr != null)
                    bossAliveImage.localPosition = BossPos;
                mapImage.localPosition = playerPos * -1.5f;
                break;
        }


        // 반투명
        //_ped.position = Input.mousePosition;
        //OnPointerEnter(_ped.position);
        //OnPointerExit(_ped.position);
    }

    private RectTransform playerImage;
    private RectTransform destinationImage;
    private RectTransform npcImage;
    private RectTransform bossAliveImage;
    private RectTransform bossDeadImage;
    private RectTransform mapImage;
    private RectTransform boarderImage1;
    private RectTransform boarderImage2;
    private RectTransform mask;
    Player player_State;

    enum size
    {
        DefaultSize,
        MiddleSize,
        MaxSize,
        //Off,
    }
    enum zoom
    {
        DefaultZoom,
        MiddleZoom,
        MaxZoom,
    }
    size curSize = size.DefaultSize;
    zoom curZoom = zoom.DefaultZoom;

    public override void Init()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            //Debug.Log("There is no player.");
            ProcessLater(() => GameObject.FindGameObjectWithTag("Player") != null, () => Init());
            return;
        }

        //base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));


        playerImage = GetObject((int)GameObjects.PlayerMarker).GetComponent<RectTransform>();
        destinationImage = GetObject((int)GameObjects.DestinationMarker).GetComponent<RectTransform>();
        bossAliveImage = GetObject((int)GameObjects.BossAliveMarker).GetComponent<RectTransform>();
        bossDeadImage = GetObject((int)GameObjects.BossDeadMarker).GetComponent<RectTransform>();
        //npcImage = GetObject((int)GameObjects.UI_NpcMarker).GetComponent<RectTransform>();
        mapImage = GetObject((int)GameObjects.MapImage).GetComponent<RectTransform>();
        boarderImage1 = GetObject((int)GameObjects.BorderImage1).GetComponent<RectTransform>();
        boarderImage2 = GetObject((int)GameObjects.BorderImage2).GetComponent<RectTransform>();
        mask = GetObject((int)GameObjects.Mask).GetComponent<RectTransform>();

        // zoom 관련 버튼 이벤트랑 묶기
        GetButton((int)Buttons.ZoomInButton).gameObject.BindEvent(OnZoomInButtonClicked);
        GetButton((int)Buttons.ZoomOutButton).gameObject.BindEvent(OnZoomOutButtonClicked);


        player_State = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();


        player = player_State.GetPlayer().transform;


        pathFinding = GameObject.Find("A*").GetComponent<PathFinding>();

        Vector3 temp = new Vector3(0, 0, 0);

        destinationImage.gameObject.SetActive(false);

        BindEvent(mapImage.gameObject, (PointerEventData data) =>
        {
            if (data.pointerId != -1) return;
            //destinationImage.transform.localPosition = new Vector3(data.position.x - 784.375f, data.position.y - 316.25f, 0); // 이거 수정하자

            Vector3 vec = new Vector3(data.position.x - playerImage.position.x, data.position.y - playerImage.position.y, 0);
            vec /= 6;

            switch (curSize)
            {
                case size.DefaultSize:
                    destinationImage.transform.localPosition = vec;
                    break;
                case size.MiddleSize:
                    destinationImage.transform.localPosition = vec / 1.3f;
                    break;
                case size.MaxSize:
                    destinationImage.transform.localPosition = vec / 1.5f;
                    break;
            }
            switch (curZoom)
            {
                case zoom.DefaultZoom:
                    break;
                case zoom.MiddleZoom:
                    destinationImage.transform.localPosition /= 1.3f;
                    break;
                case zoom.MaxZoom:
                    destinationImage.transform.localPosition /= 1.5f;
                    break;
            }

            // 수정 -> 덜덜거림
            temp.x = destinationImage.transform.localPosition.x + player.transform.position.x;
            temp.z = destinationImage.transform.localPosition.y + player.transform.position.z;
            temp.y = player.position.y; // 실제 플레이어 y값하고 차이나서 도착 거리보다 항상 커서 왔다갔다하는 문제 해결
            //temp.y = 1;
            //temp *= 6;

            player_State.Set_Destination(temp);
        }, Define.UIEvent.Click);

        // 맵 이름 초기화
        Text _MapNameText = GetText((int)Texts.MapNameText);
        string SceneName = SceneManager.GetActiveScene().name;
        if (SceneName.Contains("InGame"))
            SceneName = SceneName.Substring(6);

        //string mapName = "";
        switch (SceneName)
        {
            case "Village":
                SceneName = "Miðgarðr";
                break;
            case "Nature":
                SceneName = "Járnviðr";
                break;
            case "Desert":
                SceneName = "Múspellsheimr";
                break;
            case "Cemetery":
                SceneName = "Helheim";
                break;
        }
        _MapNameText.text = SceneName;

        // 맵 이미지
        SetMapImage(SceneName);

        images = GetComponentsInChildren<Image>();
        mapImg = GetComponentInChildren<RawImage>();

        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

        Npcs = GameObject.Find("NPC").GetComponentsInChildren<Npc>();
        foreach(Npc npc in Npcs)
        {
            
            GameObject item = Managers.UI.MakeSubItem<UI_NpcMarker>(parent: mapImage.transform).gameObject;
            UI_NpcMarker objImg = item.GetOrAddComponent<UI_NpcMarker>();
            Vector3 npcPos = npc.transform.position;
            npcPos.y = npcPos.z;
            npcPos.z = 0;
            npcPos *= 6;

            objImg.gameObject.transform.localPosition = npcPos;
            objImg.gameObject.transform.localScale = new Vector3(1, 1, 1);

            if (npc._patrolable == true)
            {
                patrolNpc.Add(npc);
                patrolNpcMarkers.Add(objImg);
            }
        }

        // find boss enemy
        if(BossTr == null)
            findBoss(); 
    }

    private void findBoss()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (go.GetComponent<EnemyBoss>() == null) continue;
            BossTr = go.transform;
            BossPos = go.transform.position;
            BossPos.y = BossPos.z;
            BossPos.z = 0;
            BossPos *= 6;

            if (go.GetComponent<EnemyStat>().Hp <= 0) // 죽은 경우
            {
                bossDeadImage.localPosition = BossPos;
                bossDeadImage.gameObject.SetActive(true);
                bossAliveImage.gameObject.SetActive(false);
            }
            else
            {
                bossAliveImage.localPosition = BossPos;
                bossAliveImage.gameObject.SetActive(true);
                bossDeadImage.gameObject.SetActive(false);
            }
        }
        // 못 찾은 경우(없는 경우)
        if(BossTr == null)
        {
            bossAliveImage.gameObject.SetActive(false);
            bossDeadImage.gameObject.SetActive(false);
        }
    }

    void RotationControl(RectTransform image)
    {
        // 회전
        Vector3 compassRotation = image.transform.eulerAngles;
        compassRotation.z = player.eulerAngles.y;
        image.transform.eulerAngles = compassRotation * -1;
    }

    UI_NewMiniMap UI_miniMap;
    private void OnPointerEnter(Vector2 pointer)
    {
        //if (UI_miniMap == RaycastAndGetFirstComponent<UI_NewMiniMap>())
        //    return;
        UI_miniMap = RaycastAndGetFirstComponent<UI_NewMiniMap>();
        RawImage rawImage = RaycastAndGetFirstComponent<RawImage>();
        Mask mask = RaycastAndGetFirstComponent<Mask>();
        if (UI_miniMap == null && rawImage == null && mask == null) return;

        //if (mask == RaycastAndGetFirstComponent<Mask>()) return;
        //mask = RaycastAndGetFirstComponent<Mask>();
        //if (mask == null) return;

        // 불투명
        foreach (Image img in images)
        {
            img.color = new Color(img.color.r , img.color.g, img.color.b, 1.0f);
        }
        mapImg.color = new Color(mapImg.color.r, mapImg.color.g, mapImg.color.b, 1.0f);
    }

    private void OnPointerExit(Vector2 pointer)
    {
        UI_NewMiniMap miniMap = RaycastAndGetFirstComponent<UI_NewMiniMap>();
        RawImage rawImage = RaycastAndGetFirstComponent<RawImage>();
        Mask mask = RaycastAndGetFirstComponent<Mask>();
        if (miniMap != null || rawImage != null || mask != null) return;

        //Mask mask = RaycastAndGetFirstComponent<Mask>();
        //if (mask != null) return;

        // 반투명
        foreach (Image img in images)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.2f);
        }
        mapImg.color = new Color(mapImg.color.r, mapImg.color.g, mapImg.color.b, 0.2f);
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
        destination = player_State.Get_Destination();

        if (destination.y >= player_State.magicNumber)
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
        destination *= 6;

        destinationImage.localPosition = destination;

        // 방향
        destinationImage.GetComponent<RectTransform>().rotation = mapImage.GetChild(0).GetComponent<RectTransform>().rotation;

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

    public void drawLine()
    {
        drawUILine = mapImage.GetComponent<DrawUILine>();

        drawUILine.ClearLine();
        destinationImage.gameObject.SetActive(false);

        List<Vector3> path = pathFinding.Return_Path(player);
        for (int i = 0; i < path.Count; i++)
        {
            path[i] *= 6;
        }

        if (destinationImage.localPosition != null && player_State.get_isObstacle() == false)
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
            drawUILine.DrawLine(playerImage.localPosition, path, player_State.get_isObstacle());
            drawDestinationMark();
            return;
        }

        if (path.Count != 0 && Vector3.Distance(playerImage.position, destinationImage.position) < arriveDist)
        {
            // path 는 있으나, 거리가 가까운 경우(?)
            drawUILine.ClearLine();
            path.Clear();
        }

    }

    public void SizeControl()
    {
        // 미니맵 사이즈 조절

        curSize = (size)(((int)curSize + 1) % 3);

        switch ((int)curSize)
        {
            case 0:
                //Zoom();
                //mapImage.localScale = new Vector3(1, 1, 0);
                mask.localScale = new Vector3(1, 1, 0);
                boarderImage1.localScale = new Vector3(1, 1, 0);
                boarderImage2.localScale = new Vector3(1, 1, 0);
                break;
            case 1:
                mask.localScale = new Vector3(1.3f, 1.3f, 0);
                boarderImage1.localScale = new Vector3(1.3f, 1.3f, 0);
                boarderImage2.localScale = new Vector3(1.3f, 1.3f, 0);
                //mapImage.localScale = new Vector3(2, 2, 0);
                break;
            case 2:
                mask.localScale = new Vector3(1.5f, 1.5f, 0);
                boarderImage1.localScale = new Vector3(1.5f, 1.5f, 0);
                boarderImage2.localScale = new Vector3(1.5f, 1.5f, 0);
                //mapImage.localScale = new Vector3(3, 3, 0);
                break;
            //case 3:
            //    //Managers.UI.ClosePopupUI();
            //    this.gameObject.SetActive(false);
            //    break;
        }

    }
    public void Zoom(bool reverse = false)
    {
        // 미니맵 줌 기능

        if (reverse)
            curZoom = (zoom)(((int)curZoom + 2) % 3);
        else
            curZoom = (zoom)(((int)curZoom + 1) % 3);

        switch ((int)curZoom)
        {
            case 0:  // default
                mapImage.localScale = new Vector3(1, 1, 0);
                playerImage.localScale = new Vector3(1, 1, 0);
                destinationImage.localScale = new Vector3(1, 1, 0);
                break;
            case 1:  // second
                mapImage.localScale = new Vector3(1.3f, 1.3f, 0);
                playerImage.localScale = new Vector3(0.77f, 0.77f, 0);
                destinationImage.localScale = new Vector3(0.77f, 0.77f, 0);
                break;
            case 2: // max
                mapImage.localScale = new Vector3(1.5f, 1.5f, 0);
                playerImage.localScale = new Vector3(0.66f, 0.66f, 0);
                destinationImage.localScale = new Vector3(0.66f, 0.66f, 0);
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
        Zoom(reverse: true);
    }


    public void SetMapImage(string mapName)
    {
        switch (mapName)
        {
            case "Miðgarðr":
                mapImage.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("Sprites/Map/Village");
                break;
            case "Járnviðr":
                mapImage.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("Sprites/Map/Nature");
                break;
            case "Múspellsheimr":
                mapImage.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("Sprites/Map/Desert");
                break;
            case "Helheim":
                mapImage.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("Sprites/Map/Cemetery");
                break;
        }
    }
}

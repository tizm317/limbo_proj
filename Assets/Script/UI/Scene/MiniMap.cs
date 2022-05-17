using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMap : UI_Scene
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
        MapImage,
        PlayerImage,
        DestinationImage,
    }

    [SerializeField] Transform player;
    public RectTransform playerImage;
    public RectTransform destinationImage;
    Player_Controller player_Controller;
    public GameObject inputManager;
    public RectTransform mapImage;


    Dictionary<int, Data.Pos> dict_pos;

    // 길찾기
    //GRID grid;
    PathFinding pathFinding;
    List<Vector3> path;

    public GameObject linePrefab;
    GameObject line;

    LineRenderer lr;

    //bool draw;


    void Start()
    {
        Init();
    }

    void Update()
    {
        // 플레이어 위치(파란색), 목적지(빨간색) 표시

        Vector3 playerPos = player.position;
       
        // 플레이어
        playerPos.y = player.position.z;
        playerPos.z = 0;
        playerImage.localPosition = playerPos;

        Vector3 destination = player_Controller.Get_Destination();
        if (destination.y != player_Controller.magicNumber)
        {

            float dist = Mathf.Abs(playerPos.magnitude - destination.magnitude);

            // 여기에 플레이어가 이동중인지를 확인하는 것도 괜찮을듯
            // 일정 거리 이내면 목적지 표시 x
            if (dist < 1.0f)
                destinationImage.gameObject.SetActive(false);
            else
                destinationImage.gameObject.SetActive(true);

            // 목적지
            destination.y = destination.z;
            destination.z = 0;
            destinationImage.localPosition = destination;

        }

        // 플레이어 원점으로 두고 나머지 이동하도록 수정
        // 플레이어 반대방향으로 지도 반대로 이동(고정된 물체 한번에 같이 이동시키기 위해서)
        mapImage.localPosition = playerPos * -1;

        // path 정보
        // 노드 연결해서 가는 길 표시?

        // 라인 그리기
        
        //lr.SetPosition(0, playerImage.localPosition);
        //lr.SetPosition(lr.positionCount - 1, destinationImage.localPosition);

        DrawUILine drawUILine = mapImage.GetComponent<DrawUILine>();

        if(destinationImage.localPosition != null)
            drawUILine.DrawLine(playerImage.localPosition, destinationImage.localPosition);

        path = pathFinding.Return_Path(player);
        //drawUILine.DrawLine(playerImage.localPosition, path);

        //lr.SetPosition(0, playerImage.position);
        //lr.SetPosition(1, destinationImage.position);

        if (path.Count != 0 )
        {
            drawUILine.DrawLine(playerImage.localPosition, path, player_Controller.get_isObstacle());
        }

        if(path.Count != 0 && Vector3.Distance(playerImage.position, destinationImage.position) < 1.0f)
        {
            drawUILine.ClearLine();
        }
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        player = GameObject.Find("Player").transform;

        //InputManager = Managers._Input;

        inputManager = GameObject.Find("InputManager");
        player_Controller = inputManager.GetComponent<Player_Controller>();

        //GameObject playerImage = Get<GameObject>((int)GameObjects.PlayerImage);

        // path 받아오려고
        pathFinding = GameObject.Find("A*").GetComponent<PathFinding>();

        Vector3 temp = new Vector3(0, 0, 0);
        //
        GameObject go = GetObject((int)GameObjects.MapImage).gameObject;
        GameObject destinationImage = GetObject((int)GameObjects.DestinationImage).gameObject;
        destinationImage.gameObject.SetActive(false);

        BindEvent(go, (PointerEventData data) =>
        {
            if (data.pointerId != -1) return;
            //destinationImage.transform.localPosition = new Vector3(data.position.x - 784.375f, data.position.y - 316.25f, 0); // 이거 수정하자
            destinationImage.transform.localPosition = new Vector3(data.position.x - playerImage.position.x, data.position.y - playerImage.position.y, 0);

            // 수정 -> 덜덜거림
            temp.x = destinationImage.transform.localPosition.x + player.transform.position.x;
            temp.z = destinationImage.transform.localPosition.y + player.transform.position.z;
            temp.y = 1;
            player_Controller.Set_Destination(temp);
        }, Define.UIEvent.Click);



        GameObject mapImage = Get<GameObject>((int)GameObjects.MapImage);
        foreach (Transform child in mapImage.transform)
        {
            if(child.GetComponent<UI_Minimap_ObjImg>() == true)
                Managers.Resource.Destroy(child.gameObject);
        }

        // 맵 데이터
        // static object data
        dict_pos = Managers.Data.PosDict;



        // 맵 오브젝트 개수만큼 붙이기
        for (int i = 0; i < dict_pos.Count; i++) // 실제 인벤토리 정보 참고해서 몇개 만들지 고려
        {
            // 1. UI_Minimap_ObjImag 생성해서 MapImage 산하에 붙임
            GameObject item = Managers.UI.MakeSubItem<UI_Minimap_ObjImg>(parent: mapImage.transform).gameObject;


            // 1번째 방법 - 코드로 추가 : Util.GetOrAddComponent<UI_Inven_Item>(item);
            UI_Minimap_ObjImg objImg = item.GetOrAddComponent<UI_Minimap_ObjImg>(); // Extension 사용

            // 위치 설정

            Vector3 tempVec;
            tempVec.x = dict_pos[i].x;
            tempVec.y = dict_pos[i].z;
            tempVec.z = 0;
            Debug.Log(tempVec);
            objImg.gameObject.transform.localPosition = tempVec;

            //invenItem.SetInfo($"집행검{i}번");
        }

        // 길찾기
        //lr = playerImage.gameObject.GetComponent<LineRenderer>();
        //lr.startColor = Color.black;
        //lr.endColor = Color.black;
        //lr.startWidth = 1f;
        //lr.endWidth = 1f;


        //line = Instantiate(linePrefab);
        //lr = line.GetComponent<LineRenderer>();
        //lr.positionCount = 2;

    }
}

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
    
    void Start()
    {
        Init();
    }

    void Update()
    {
        // 플레이어 위치(파란색), 목적지(빨간색) 표시

        //Vector3 playerPos = player.position;
        //Vector3 destination = player_Controller.destination;
        //float dist = Mathf.Abs(playerPos.magnitude - destination.magnitude);

        //// 여기에 플레이어가 이동중인지를 확인하는 것도 괜찮을듯
        //// 일정 거리 이내면 목적지 표시 x
        //if (dist < 1.0f)
        //    destinationImage.gameObject.SetActive(false);
        //else
        //    destinationImage.gameObject.SetActive(true);

        //// 플레이어
        //playerPos.y = player.position.z;
        //playerPos.z = 0;
        //playerImage.localPosition = playerPos;

        //// 목적지
        //destination.y = player_Controller.destination.z;
        //destination.z = 0;
        //destinationImage.localPosition = destination;

        // 반대로 미니맵 클릭해서 이동하는 방식도 있으면 좋을듯?
        // UI 에 레이케스트 해서 
        // 거꾸로 
    }

    public override void Init()
    {
        base.Init();

        //Bind<GameObject>(typeof(GameObjects));

        //player = GameObject.Find("Player").transform;

        ////InputManager = Managers._Input;

        //inputManager = GameObject.Find("InputManager");
        //player_Controller = inputManager.GetComponent<Player_Controller>();

        ////GameObject playerImage = Get<GameObject>((int)GameObjects.PlayerImage);

        //Vector3 temp = new Vector3(0, 0, 0);
        ////
        //GameObject go = GetObject((int)GameObjects.MapImage).gameObject;
        //GameObject destinationImage = GetObject((int)GameObjects.DestinationImage).gameObject;
        //BindEvent(go, (PointerEventData data) => { destinationImage.transform.localPosition = new Vector3 (data.position.x - 784.375f, data.position.y - 316.25f, 0); 
        //    temp.z = destinationImage.transform.localPosition.y;
        //    temp.x = destinationImage.transform.localPosition.x;
        //    player_Controller.Set_Destination(temp); }, Define.UIEvent.Click);
    }
}

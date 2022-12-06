using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CharacterInfo
{
    public string nic_name;
    public Define.Job job;
    public PlayerStat stat;

    public CharacterInfo(string _nic_name = "Player", Define.Job _job = Define.Job.WARRIOR, PlayerStat _stat = null)
    {
        nic_name = _nic_name;
        job = _job;
        if(_stat == null)
        {
            stat = new PlayerStat();
        }
        else
            stat = _stat;
    }
}


public class LobbyScene : BaseScene
{
    // Start is called before the first frame update
    //public static List<CharacterInfo> my_list = new List<CharacterInfo>();
    public static List<LobbyPlayerInfo> lobbyPlayerlist = new List<LobbyPlayerInfo>();
    //public List<CharacterInfo> my_list = new List<CharacterInfo>();
    //public static CharacterInfo my_character_info = new CharacterInfo();
    int idx = -1;

    void Start()
    {
        SceneType = Define.Scene.Lobby;
        //Init();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Init()
    {
        //GetInfo();
        base.Init();
        Transform ui_base = GameObject.Find("Characters Group").transform;
        if(ui_base.childCount != 0)
        {
            Debug.Log(ui_base.childCount);
            for(int i = 0; i < ui_base.childCount; i++)
            {
                Destroy(ui_base.transform.GetChild(i));//리스트 전부 비워주기
            }
        }
        
        foreach(LobbyPlayerInfo i in lobbyPlayerlist)
        {
            GameObject ci = Instantiate(Resources.Load<GameObject>("RPG and MMO UI 11/Prefabs/Lobby/Character Select/Character (List)"));
            ci.transform.SetParent(ui_base);
            ci.transform.Find("Name Text").GetComponent<Text>().text = i.Name;
            ci.transform.transform.Find("Sub Texts Group").transform.Find("Level Text").GetComponent<Text>().text = i.PlayerStatInfo.Level.ToString();
            ci.transform.transform.Find("Sub Texts Group").transform.Find("Class Text").GetComponent<Text>().text = i.Job.ToString();
            ci.transform.localScale = Vector3.one;

            ci.name = i.Name;
            ci.BindEvent(Toggled,Define.UIEvent.Click);

        }
        
        //foreach(CharacterInfo i in my_list)
        //{
        //    GameObject ci = Instantiate(Resources.Load<GameObject>("RPG and MMO UI 11/Prefabs/Lobby/Character Select/Character (List)"));
        //    ci.transform.SetParent(ui_base);
        //    ci.transform.Find("Name Text").GetComponent<Text>().text = i.nic_name;
        //    ci.transform.transform.Find("Sub Texts Group").transform.Find("Level Text").GetComponent<Text>().text = i.stat.Level.ToString();
        //    ci.transform.transform.Find("Sub Texts Group").transform.Find("Class Text").GetComponent<Text>().text = i.job.ToString();
        //    ci.transform.localScale = Vector3.one;
        //    ci.name = i.nic_name;
        //}
        
    }

    public void GetInfo(S_Login loginPacket)
    {
        //TODO 웹서버에서 닉네임이 가지고 있는 정보 가져와서 사용 가능한 형태로 저장하기

        //TEMP
        //my_list.Add(new CharacterInfo("춘식이"));
        //my_list.Add(new CharacterInfo("똥개"));
        //my_list.Add(new CharacterInfo("라이언"));

        foreach(LobbyPlayerInfo playerInfo in loginPacket.Players)
        {
            lobbyPlayerlist.Add(playerInfo);
        }

        Init();
    }

    public void Toggled(PointerEventData data)
    {
        Transform ui_base = GameObject.Find("Characters Group").transform;
        int _idx = -1;
        for(int i = 0; i < ui_base.childCount; i++)
        {
            if(ui_base.transform.GetChild(i).GetComponent<Toggle>().isOn && i != idx)
                _idx = i;
        }
        if(_idx != -1)
        {
            if(idx != -1)
                ui_base.transform.GetChild(idx).GetComponent<Toggle>().isOn = false;
            ui_base.transform.GetChild(_idx).GetComponent<Toggle>().isOn = true;
            idx = _idx;
        }
        else
        {
            if(idx != -1)
                ui_base.transform.GetChild(idx).GetComponent<Toggle>().isOn = false;
            idx = -1;
        }
    }

    public void Play()
    {
        if(idx != -1)//선택을 한 경우만
        {
            LobbyPlayerInfo info = lobbyPlayerlist[idx];
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = info.Name;
            Managers.Network.Send(enterGamePacket);

            Managers.Scene.LoadScene(Define.Scene.InGameVillage);
            //my_character_info = my_list[idx];
            //LoadingScene.LoadScene(Define.Scene.InGameVillage);//마지막 위치에서 소환되게하려면 여기서 수정~
        }
    }

    public void CreateCharacterScene()
    {
        LoadingScene.LoadScene(Define.Scene.CharacterCreate);
    }

    public override void Clear()
    {

    }
}

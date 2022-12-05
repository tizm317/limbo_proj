using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public static List<CharacterInfo> my_list = new List<CharacterInfo>();
    void Start()
    {
        SceneType = Define.Scene.Lobby;
        Init();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Init()
    {
        GetInfo();
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
        
        foreach(CharacterInfo i in my_list)
        {
            GameObject ci = Instantiate(Resources.Load<GameObject>("RPG and MMO UI 11/Prefabs/Lobby/Character Select/Character (List)"));
            ci.transform.SetParent(ui_base);
            ci.transform.Find("Name Text").GetComponent<Text>().text = i.nic_name;
            ci.transform.transform.Find("Sub Texts Group").transform.Find("Level Text").GetComponent<Text>().text = i.stat.Level.ToString();
            ci.transform.transform.Find("Sub Texts Group").transform.Find("Class Text").GetComponent<Text>().text = i.job.ToString();
            ci.transform.localScale = Vector3.one;
            ci.name = i.nic_name;
        }
        
    }

    public void GetInfo()
    {
        //TODO 웹서버에서 닉네임이 가지고 있는 정보 가져와서 사용 가능한 형태로 저장하기

        //TEMP
        my_list.Add(new CharacterInfo("춘식이"));
        my_list.Add(new CharacterInfo("똥개"));
        my_list.Add(new CharacterInfo("라이언"));

    }

    public void Toggled(int idx)
    {
        Transform ui_base = GameObject.Find("Characters Group").transform;
        for(int i = 0; i < ui_base.childCount; i++)
        {

        }
    }

    public void Play()
    {

    }

    public void CreateCharacterScene()
    {
        LoadingScene.LoadScene(Define.Scene.CharacterCreate);
    }

    public override void Clear()
    {

    }
}

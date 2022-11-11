using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMgr:MonoBehaviour//Managers가 만약 Ingame에서 생성되는 거라면? Instance로 추가, 아니라면 Singleton을 Awake에 추가해주어야함
{
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


    // Start is called before the first frame update
    public Define.Job job;
    SkillData[] skillDatas = new SkillData[5];
    GameObject playerGO;
    Player ps;
    string my_name;
    [SerializeField]
    Vector3 pos;
    protected Vector3 start_pos = new Vector3(1.2f, 1f, -62.6f);
    [SerializeField]
    GameObject[] character;
    void Awake()
    {
        Init();
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    void GetInfo()
    {
        //캐릭터 종류와 데이터를 받아오는 내용이 필요함
        character = Resources.LoadAll<GameObject>("Prefabs/Character");
        my_name = "Player";
        if(pos == null || pos ==Vector3.zero)
            pos = start_pos;
    }

    void Init()
    {
        //var obj = GameObject.FindGameObjectsWithTag("Player");//플레이어가 있다면(서버넘어가면 수정해야할 내용일듯?)
        //if(obj.Length == 1)//이미 있어?
        //{
        //    ps = obj[0].GetComponent<Player>();
        //    //job = ps.JOB;
        //    my_name = obj[0].name;
        //    Camera.main.GetComponent<Camera_Controller>().SetTarget(obj[0]);
        //}
        //else
        //{
        //    GetInfo();
        //    //캐릭터 종류와 데이터에 따라서 새로운 Instantiate해주는 내용이 필요함
        //    switch(job)
        //    {
        //        case Define.Job.WARRIOR :
        //            gameObject.AddComponent<Warrior>();            
        //            break;

        //        case Define.Job.ARCHER :
        //            gameObject.AddComponent<Archer>();
        //            break;

        //        case Define.Job.SORCERER :
        //            gameObject.AddComponent<Sorcerer>();
        //            break;

        //        default :
        //            gameObject.AddComponent<Warrior>();    
        //            break;
        //    }
        //    GameObject temp = GameObject.Instantiate<GameObject>(character[(int)job]);
        //    temp.name = my_name;
        //    temp.transform.position = pos;

        //    DontDestroyOnLoad(temp);
        //    ps = gameObject.GetComponent<Player>();
        //    ps.SetPlayer(temp);

        //    Camera.main.GetComponent<Camera_Controller>().SetTarget(temp);
        //}

        //// 한번만 (서버 안쓸때 임시)
        // 플레이어 초기화
        if (Managers.Object.MyPlayer == null)
        {
            // playerMgr 에 public으로 셋팅된 직업
            PlayerInfo info = new PlayerInfo() { Name = "MyPlayer", PlayerId = 0, PosInfo = new PositionInfo(), DestInfo = new PositionInfo(), Job = (int)job };
            Managers.Object.Add(info, myPlayer: true);
        }

        // 서버에서
        playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null)
        {
            // player 가 아직 생성 전이면, 생긴 이후에 다시 Init하도록 코루팀으로 대기함
            ProcessLater(() => GameObject.FindGameObjectWithTag("Player") != null, () => Init());
            return;
        }
        ps = playerGO.GetComponent<Player>();
        job = ps.my_job;
        GameObject skill_ui_root = GameObject.Find("Grid");
        Sprite[] skill_img = new Sprite[5];
        skill_img = Resources.LoadAll<Sprite>("Skill_Sprite/" + job.ToString());

        // Skill Data
        
        skillDatas[0] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_P");
        skillDatas[1] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_Q");
        skillDatas[2] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_W");
        skillDatas[3] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_E");
        skillDatas[4] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_R");
        ps.skillDatas = skillDatas;

        for (int i = 0; i < ps.Skill_img.Length; i++)
        {
            ps.Skill_img[i] = skill_ui_root.transform.GetChild(i).transform.GetChild(3).GetComponent<Image>();
            ps.Skill_img[i].gameObject.SetActive(true);
            ps.Skill_img[i].sprite = skill_img[i];
        }
        
    }

    public void ToolTip()
    {
        switch (job)
        {
            case Define.Job.WARRIOR:
                skillDatas[0].Name = "";
                skillDatas[1].Name = "";
                skillDatas[2].Name = "";
                skillDatas[3].Name = "";
                skillDatas[4].Name = "";
                skillDatas[0].Tooltip = $"매초 잃은 체력의 1%를 회복합니다.";
                skillDatas[1].Tooltip = $"도끼를 크게 휘둘러 전방의 {ps.attackRange * 1.5f}만큼의 거리에 {ps.my_stat.Attack * (1 + ps.skill_level[0] * 0.25f)}의 데미지를 줍니다.";
                skillDatas[2].Tooltip = $"{ps.attackRange * (2 + ps.skill_level[1])}거리 이내의 적들을 도발하고, 5초간 체력 재생력이 {1 + ps.skill_level[1]}배 만큼 증가합니다.";
                skillDatas[3].Tooltip = $"힘찬 함성으로 주위 적들을 꾸짖어 5초간 {ps.attackRange * (2 + ps.skill_level[2])}거리 이내의 적들의 공격력과 공격속도를 {5f * ps.skill_level[2]}%만큼 감소시킵니다.";
                skillDatas[4].Tooltip = $"도약한 뒤 지면을 내려쳐 {1 + ps.skill_level[3]}거리 이내의 적들에게 {ps.my_stat.Attack * (ps.skill_level[3] * 0.5f + 1f)}만큼의 데미지를 줍니다.";
                break;
            case Define.Job.ARCHER:
                skillDatas[0].Name = "";
                skillDatas[1].Name = "";
                skillDatas[2].Name = "";
                skillDatas[3].Name = "";
                skillDatas[4].Name = "";
                skillDatas[0].Tooltip = $"레벨당 사거리가 0.1씩 증가합니다(현재 증가량 : {(ps.my_stat.Level - 1f) * 0.1f}";
                skillDatas[1].Tooltip = $"궁수가 적의 공격을 신속히 회피합니다(짧은 시간동안 무적상태가 됩니다.)";
                skillDatas[2].Tooltip = $"비전 약물을 투여하여 {(int)(4f + ps.skill_level[1])}초간 공격속도가 {50f + ps.skill_level[1] * 10f}퍼센트 만큼 증가합니다.";
                skillDatas[3].Tooltip = $"마나를 담은 화살을 넓은 범위로 발사하여 {ps.attackRange}거리의 적들에게 {ps.my_stat.Attack * (1f + ps.skill_level[2] * 0.25f)}의 데미지를 줍니다.";
                skillDatas[4].Tooltip = $"화살 한발에 최대한 많은 마나를 담아 발사합니다. 화살은 경로상의 모든 적들에게 {ps.my_stat.Attack * 5f}의 데미지를 줍니다.";
                break;
            case Define.Job.SORCERER:
                skillDatas[0].Name = "";
                skillDatas[1].Name = "";
                skillDatas[2].Name = "";
                skillDatas[3].Name = "";
                skillDatas[4].Name = "";
                skillDatas[0].Tooltip = $"공격에 5의 마나를 소모하여 적에게 더 큰 피해를 줍니다.";
                skillDatas[1].Tooltip = $"마법사가 안개속에 숨어 {4 + ps.skill_level[0]}초간 적들이 찾을 수 없습니다.";
                skillDatas[2].Tooltip = $"마법사가 아군의 체력을 {ps.my_stat.Attack}만큼 회복합니다.";
                skillDatas[3].Tooltip = $"마법사가 마법으로 인챈트된 냉기의 검을 소환하여 {ps.skill_level[2] + 1}범위의 적들의 이동속도를 {20f + ps.skill_level[2] * 5f}만큼 감소시키고 {ps.my_stat.Attack * (1 + ps.skill_level[2] * 0.25f)}만큼 데미지를 줍니다.";
                skillDatas[4].Tooltip = $"마법사가 마법으로 인챈트된 방패를 소환하여 {1 + ps.skill_level[3] + 3}범위의 적들을 가두고 갇힌 적에게는 {ps.my_stat.Attack * (1f + ps.skill_level[3] * 0.5f)}만큼의 데미지를 주고 아군을 회복합니다.";
                break;
        }
    }
}

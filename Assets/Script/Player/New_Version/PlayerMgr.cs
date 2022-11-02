using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMgr:MonoBehaviour//Managers가 만약 Ingame에서 생성되는 거라면? Instance로 추가, 아니라면 Singleton을 Awake에 추가해주어야함
{
    // Start is called before the first frame update
    public Define.Job job;
    SkillData[] skillDatas = new SkillData[5];
    Player ps;
    string my_name;
    [SerializeField]
    Vector3 pos;
    protected Vector3 start_pos = new Vector3(1.2f,1f,-62.6f);
    [SerializeField]
    GameObject[] character;
    void Awake()
    {
        GetInfo();
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
        //캐릭터 종류와 데이터에 따라서 새로운 Instantiate해주는 내용이 필요함
        switch(job)
        {
            case Define.Job.WARRIOR :
                gameObject.AddComponent<Warrior>();            
                break;

            case Define.Job.ARCHER :
                gameObject.AddComponent<Archer>();
                break;

            case Define.Job.SORCERER :
                gameObject.AddComponent<Sorcerer>();
                break;

            default :
                gameObject.AddComponent<Warrior>();    
                break;
        }
        GameObject temp = GameObject.Instantiate<GameObject>(character[(int)job]);
        temp.name = my_name;
        temp.transform.position = pos;
        ps = gameObject.GetComponent<Player>();
        ps.SetPlayer(temp);
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
            ps.Skill_img[i] = skill_ui_root.transform.GetChild(i).transform.GetChild(2).GetComponent<Image>();
            ps.Skill_img[i].gameObject.SetActive(true);
            ps.Skill_img[i].sprite = skill_img[i];
        }
        Camera.main.GetComponent<Camera_Controller>().SetTarget(temp);
    }

    public void ToolTip()
    {
        switch (job)
        {
            case Define.Job.WARRIOR:
                skillDatas[0].Tooltip = $"매초 잃은 체력의 1%를 회복한다";
                skillDatas[1].Tooltip = $"도끼를 크게 휘둘러 전방의 {ps.attackRange * 1.5f}만큼의 거리에 {ps.my_stat.Attack * (1 + ps.skill_level[0] * 0.25f)}의 데미지를 준다.";
                skillDatas[2].Tooltip = $"{ps.attackRange * (2 + ps.skill_level[1])}거리 이내의 적들을 도발하고, 5초간 체력 재생력을 {1 + ps.skill_level[1]}배 만큼 증가시킨다.";
                skillDatas[3].Tooltip = $"힘찬 함성으로 주위 적들을 꾸짖어 5초간 {ps.attackRange * (2 + ps.skill_level[2])}거리 이내의 적들의 공격력과 공격속도를 {5f * ps.skill_level[2]}%만큼 감소시킨다.";
                skillDatas[4].Tooltip = $"도약한 뒤 지면을 내려쳐 {1 + ps.skill_level[3]}거리 이내의 적들에게 {ps.my_stat.Attack * (ps.skill_level[3] * 0.5f + 1f)}만큼의 데미지를 준다.";
                break;
            case Define.Job.ARCHER:
                skillDatas[0].Tooltip = $"매초 잃은 체력의 1%를 회복한다";
                skillDatas[1].Tooltip = $"도끼를 크게 휘둘러 전방의 {ps.attackRange * 1.5f}만큼의 거리에 {ps.my_stat.Attack * (1 + ps.skill_level[0] * 0.25f)}의 데미지를 준다.";
                skillDatas[2].Tooltip = $"";
                skillDatas[3].Tooltip = $"";
                skillDatas[4].Tooltip = $"";
                break;
            case Define.Job.SORCERER:
                skillDatas[0].Tooltip = $"매초 잃은 체력의 1%를 회복한다";
                skillDatas[1].Tooltip = $"도끼를 크게 휘둘러 전방의 {ps.attackRange * 1.5f}만큼의 거리에 {ps.my_stat.Attack * (1 + ps.skill_level[0] * 0.25f)}의 데미지를 준다.";
                skillDatas[2].Tooltip = $"";
                skillDatas[3].Tooltip = $"";
                skillDatas[4].Tooltip = $"";
                break;
        }
    }
}

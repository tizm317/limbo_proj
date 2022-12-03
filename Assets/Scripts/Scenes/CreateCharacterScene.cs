using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CreateCharacterScene : BaseScene
{
    // Start is called before the first frame update
    Camera[] Cameras = new Camera[3];
    [SerializeField] Image shade_img;
    [SerializeField] InputField input_name;
    [SerializeField] Image[] imgs = new Image[5];
    [SerializeField] Text[] texts = new Text[5];
    float x_cam;
    float z_cam;
    float r;

    enum Buttons
    {
        Warrior,
        Archer,
        Sorcerer,
    }
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Init()
    {
        SceneType = Define.Scene.CharacterCreate;
        base.Init();

        SetCamera();

        Transform info_base = GameObject.Find("Canvas (Overlay)").transform.GetChild(0).transform.GetChild(0);
        for(int i = 0; i < 5; i++)
        {
            imgs[i] = info_base.GetChild(i).GetComponent<Image>();
            texts[i] = imgs[i].transform.GetChild(0).GetComponent<Text>();
        }

        Character_Select(1);
    }

    void SetCamera()
    {
        var characters = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < Cameras.Length; i++)
        {
            Cameras[i] = characters[i].transform.GetChild(characters[i].transform.childCount -1).GetComponent<Camera>();
            Cameras[i].transform.localPosition = new Vector3(0f, 1.2f, 3f);
            Cameras[i].transform.LookAt(characters[i].transform.GetChild(0)); // Hips 기준
        }
    }

    public void Character_Select(int idx)
    {
        shade_img.gameObject.SetActive(false);
        for(int i = 0; i < Cameras.Length; i++)
        {
            if(i != idx)
            {
                Cameras[i].gameObject.SetActive(false);
            }
            else
            {
                Cameras[i].gameObject.SetActive(true);
            }
        }
        string job = "";
        switch(idx)
        {
            case 0:
                job = "WARRIOR";
                //texts = 
                break;
            case 1:
                job = "ARCHER";
                break;
            case 2:
                job = "SORCERER";
                break;
        }
        SkillData[] skillDatas = new SkillData[5];
        skillDatas[0] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_P");
        skillDatas[1] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_Q");
        skillDatas[2] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_W");
        skillDatas[3] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_E");
        skillDatas[4] = Resources.Load<SkillData>($"Prefabs/Skill/{job}/Skill_R");
        for(int i = 0; i < 5; i++)
        {
            imgs[i].sprite = skillDatas[i].IconSprite;
            texts[i].text = skillDatas[i].Tooltip;
        }
        PlayerMgr.cur_JOB = (Define.Job)idx;//생성 전에 직업 변경해버리기~
    }

    public void Create_Character()
    {
        //TODO 캐릭터 이름 받은 거랑 플레이어 스텟이랑 직업 이용해서 새 캐릭터 만드는 부분 추가
        PlayerMgr.Nickname = input_name.text;
        LoadingScene.LoadScene(Define.Scene.InGameVillage);  
    }

    public void Back(string scenename)
    {
        LoadingScene.LoadScene(scenename);
    }

    public override void Clear()
    {
        Debug.Log("CharacterCreateScene Clear");
    }
}

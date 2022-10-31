using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMgr:MonoBehaviour//Managers가 만약 Ingame에서 생성되는 거라면? Instance로 추가, 아니라면 Singleton을 Awake에 추가해주어야함
{
    // Start is called before the first frame update
    public Define.Job job;
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
        Player ps = gameObject.GetComponent<Player>();
        ps.SetPlayer(temp);
        GameObject skill_ui_root = GameObject.Find("Grid");
        Sprite[] skill_img = new Sprite[5];
        skill_img = Resources.LoadAll<Sprite>("Skill_Sprite/" + job.ToString());
        
        for(int i = 0; i < ps.Skill_img.Length; i++)
        {
            ps.Skill_img[i] = skill_ui_root.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>();
            ps.Skill_img[i].gameObject.SetActive(true);
            ps.Skill_img[i].sprite = skill_img[i];
        }
        Camera.main.GetComponent<Camera_Controller>().SetTarget(temp);
    }
}

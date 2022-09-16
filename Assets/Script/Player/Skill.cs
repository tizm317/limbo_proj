using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Skill : MonoBehaviour
{
    // Start is called before the first frame update
    Effect effect;
    Player_State player_state;
    [SerializeField]
    public float[] cool = new float[4];
    private float[] cool_max = new float[4];
    private bool[] isActive = new bool[4];
    public Image[] Skill_img = new Image[4];
    void Start()
    {
        Init();
    }

    void Update()
    {
        Cool();
    }

    void Init()
    {
        effect = gameObject.GetComponent<Effect>();
        player_state = gameObject.GetComponent<Player_State>();
        cool_max[0] = 5f;
        cool_max[1] = 4f;
        cool_max[2] = 3f;
        cool_max[3] = 15f;
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
            isActive[i] = true;
        }
    }

    void Cool()
    {
        for(int i = 0; i < cool.Length; i++)
        {
            if(isActive[i])
            {
                if(cool[i] > 0)
                    cool[i] -= Time.deltaTime;
                else if(cool[i] < 0)
                    cool[i] = 0;
            }
            //Skill_img[i].fillAmount = cool_max[i] - cool[i]/cool_max[i];
        }
    }

    public void Q()//가시용 my_job으로 직업에 따라 배정되는 스킬을 달라지게 함
    {
        switch(player_state.job)
        {
            case "Warrior" :
                if(cool[0] == 0)
                    StartCoroutine(Warrior_Q(45f,5f,15f));
                break;
        }
    }

    public void W()
    {
        switch(player_state.job)
        {
            case "Warrior" :
                if(cool[1] == 0)
                    StartCoroutine(Warrior_W(2f,5f));
                break;
        }
    }

    public void E()
    {
        switch(player_state.job)
        {
            case "Warrior" :
                if(cool[2] == 0)
                    StartCoroutine(Warrior_E());
                break;
        }
    }

    public void R()
    {
        switch(player_state.job)
        {
            case "Warrior" :
                if(cool[3] == 0)
                    StartCoroutine(Warrior_R(2f,5f));
                break;
        }
    }

    IEnumerator Warrior_Q(float SightAngle, float distance,float damage)
    {
        bool canceled = false;
        bool pos_selected = false;
        Vector3 pos;
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        Debug.Log(pos);
                        gameObject.transform.forward = new Vector3(pos.x - gameObject.transform.position.x, 0, pos.z - gameObject.transform.position.z).normalized;
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.R))
                {
                    canceled = true;
                    player_state.on_skill = false;
                }
                yield return new WaitForEndOfFrame();
            }
            if(pos_selected)
            {
                player_state.curState = Player_State.State.STATE_SKILL;
                player_state.skill = Player_State.HotKey.Q;
                player_state.Ani_State_Change();
                yield return new WaitForSeconds(2.2f);

                for(int i = 0; i < player_state.enemies.Count; i++)
                {
                    Vector3 targetDir = (player_state.enemies[i].transform.position - gameObject.transform.position).normalized;//방향 계산

                    float dot = Vector3.Dot(gameObject.transform.forward, targetDir);//내적 계산

                    float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;//내적과 acos을 이용하여 사이각 계산

                    float far = Vector3.Distance(gameObject.transform.position, player_state.enemies[i].transform.position);

                    if(theta <= SightAngle && far < distance)
                        player_state.enemies[i].GetComponent<Stat>().Hp -= damage;
                }
                cool[0] = cool_max[0];
                player_state.on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Warrior_W(float mul, float time)
    {
        //mul = 체젠 배율
        //time = 얼마나 오랫동안?
        player_state.curState = Player_State.State.STATE_SKILL;
        player_state.skill = Player_State.HotKey.W;
        player_state.Ani_State_Change();
        yield return new WaitForSeconds(2.5f);//애니메이션을 시전하는 동안

        player_state.my_stat.Regeneration *= mul;
        cool[1] = cool_max[1];
        player_state.on_skill = false;
        //도발에 대한 내용이 있어야함 - 희진누나랑 상담해볼 부분
        yield return new WaitForSeconds(time);
        player_state.my_stat.Regeneration /= mul;
        
        
    }
    IEnumerator Warrior_E()
    {
        player_state.curState = Player_State.State.STATE_SKILL;
        player_state.skill = Player_State.HotKey.E;
        player_state.Ani_State_Change();
        yield return new WaitForSeconds(1f);
        Managers.Sound.Play("Sound/warrior_yelling",Define.Sound.Effect, 1.0f, true);
        yield return new WaitForSeconds(4f);
        //적의 공격속도와 공격력을 감소시키는 내용이 있어야함 - 희진누나랑 상담해볼 부분
        cool[2] = cool_max[2];
        player_state.on_skill = false;
    }

    IEnumerator Warrior_R(float range, float damage)
    {
        bool canceled = false;
        bool pos_selected = false;
        Vector3 pos;
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        Debug.Log(pos);
                        gameObject.transform.forward = new Vector3(pos.x - gameObject.transform.position.x, 0, pos.z - gameObject.transform.position.z).normalized;
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.Q))
                {
                    canceled = true;
                    player_state.on_skill = false;
                }
                yield return new WaitForEndOfFrame();
            }
            if(pos_selected)
            {
                player_state.curState = Player_State.State.STATE_SKILL;
                player_state.skill = Player_State.HotKey.R;
                player_state.Ani_State_Change();
                yield return new WaitForSeconds(2.2f);

                for(int i = 0; i < player_state.enemies.Count; i++)
                {
                    float far = Vector3.Distance(gameObject.transform.position, player_state.enemies[i].transform.position);

                    if(far < range)
                        player_state.enemies[i].GetComponent<Stat>().Hp -= damage;
                }
                cool[3] = cool_max[3];
                player_state.on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Skill : MonoBehaviour
{
    // Start is called before the first frame update
    Effect effect;
    Player_State player_state;
    public enum Job
    {
        Warrior,
        Archer,
        Magician,
    }
    [SerializeField]
    public float[] cool = new float[4];
    private float[] cool_max = new float[4];
    private bool[] isActive = new bool[4];
    public Image[] Skill_img = new Image[4];
    static Job my_job;
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
        my_job = Job.Warrior;
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
        switch(my_job)
        {
            case Job.Warrior :
                if(cool[0] == 0)
                    StartCoroutine(Warrior_Q(45f,5f,15f));
                break;
        }
    }

    public void W()
    {

    }

    public void E()
    {

    }

    public void R()
    {

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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Archer : Player
{
    GameObject Arrow;
    GameObject R_Arrow, skill_Arrow;
    GameObject W_Effect;
    Buff _buff;
    public override void abstract_Init()
    {
        job = "Archer";
        Arrow = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Arrow");
        R_Arrow = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/ElementalArrow2");
        skill_Arrow = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/ElementalArrow2_Small");
        W_Effect = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Archer_W");
        attackRange = 15f;
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
        }
        _buff = gameObject.AddComponent<Buff>();
    }

    public override void Cool_Update()
    {
       cool_max[0] = 10f - (2f * skill_level[0] -1);
       cool_max[1] = 20f - (2f * skill_level[1] - 1);
       cool_max[2] = 10f - (2f * skill_level[2] -1);
       cool_max[3] = 40 - 4f * skill_level[3];
    }

    public override void Attack()
    {
        if(my_enemy == null)
        {
            curState = State.STATE_IDLE;
            Ani_State_Change();
            return;
        }
        if(Vector3.Distance(player.transform.position, my_enemy.transform.position) > attackRange)//적이 사거리 이내에 있는지 확인 조건, 아니라면 적 방향으로 이동
        {
            Vector3 dir = (player.transform.position - my_enemy.transform.position).normalized * attackRange;
            Set_Destination(my_enemy.transform.position - dir);
            curState = State.STATE_MOVE;
            Ani_State_Change();
        }
        else
        {
            if(!isAttack)//공격을 이미 실행중이지 않은 경우에만 작동
            {
                StartCoroutine(Attack(my_stat.AttackSpeed, 15f));
            }
        }
    }

    IEnumerator Attack(float attack_speed, float flight_speed)//공격 함수 구현부
    {
        Vector3 pos = player.transform.GetChild(2).position;//화살이 활에서 나가도록 조정해주어야함
        if(!on_skill)//스킬을 사용중이라면 공격할 수 없음
        {
            isAttack = true;
            isMove = false;

            curState = State.STATE_ATTACK;
            //Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
            Ani_State_Change();
            player.transform.forward = (my_enemy.transform.position - player.transform.position).normalized;
            ani.SetFloat("AttackSpeed",attack_speed);
            yield return new WaitForSeconds(3.8f/attack_speed);//공격 애니메이션 시간
            
            if(my_enemy != null)
            {
                GameObject temp = GameObject.Instantiate<GameObject>(Arrow);
                temp.transform.position = pos;
                //이곳에 화살을 소모하는 내용이 필요함 
                temp.GetComponent<Arrow>().Arrow_(my_enemy,15f,this);
            }
            
            curState = State.STATE_IDLE;
            Ani_State_Change();
            yield return new WaitForSeconds(1/attack_speed);//1초를 공격속도로 나눈 값만큼 기다렸다가 다음 공격을 수행
            isAttack = false;     
        }
    }

    public override void Q()
    {
        StartCoroutine(Archer_Q());
    }

    public override void W()
    {
        StartCoroutine(Archer_W());
    }

    public override void E()
    {
        StartCoroutine(Archer_E());
    }

    public override void R()
    {
        StartCoroutine(Archer_R());
    }

    public override void Passive()
    {
        StartCoroutine(Archer_Passive());
    }

    IEnumerator Archer_Q()
    {
        pos_selected = false;
        canceled = false;
        Vector3 pos;
        StartCoroutine(Show_ArrowIndicator(true,5));//range값은 5넘어가면 안됨
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    destination.Clear();
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        player.transform.forward = new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized;
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.R))
                {
                    canceled = true;
                    on_skill = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            if(pos_selected)
            {
                curState = State.STATE_SKILL;
                skill = HotKey.Q;
                Ani_State_Change();
                attackable = false;
                yield return new WaitForSeconds(1.08f);
                attackable = true;
                
                cool[0] = cool_max[0];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }

    IEnumerator Archer_W()
    {
        float speed = 50f + skill_level[1] * 10f;
        float time = 4f + skill_level[1];
        my_stat.AttackSpeed *= (1f + speed/100f);
        GameObject temp = Instantiate<GameObject>(W_Effect);
        temp.transform.SetParent(player.transform);
        temp.transform.localPosition = Vector3.zero;
        _buff.Show_buff(time,player,Skill_img[2].sprite);
        on_skill = false;
        
        cool[1] = cool_max[1];//시전시간이 없어서 일단은 바로 쿨 돌리기
        yield return new WaitForSeconds(time);//지속시간
        Destroy(temp);
        my_stat.AttackSpeed /= (1f + speed/100f);;
    }

    IEnumerator Archer_E()
    {
        float SightAngle = 60f;
        float distance = attackRange;
        float damage = my_stat.Attack * (1f + skill_level[2] * 0.25f);
        pos_selected = false;
        canceled = false;
        Vector3 pos;
        StartCoroutine(Show_CircleIndicator(true,SightAngle,distance));
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    destination.Clear();
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        player.transform.forward = new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized;
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.Q)||Input.GetKey(KeyCode.R))
                {
                    canceled = true;
                    on_skill = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if(pos_selected)
            {
                curState = State.STATE_SKILL;
                skill = HotKey.E;
                Ani_State_Change();
                Vector3 pos_s = player.transform.GetChild(2).position;
                yield return new WaitForSeconds(1.8f);
                List<GameObject> temp = new List<GameObject>();
                int how_many = (int)(SightAngle/10f);
                //이곳에 스킬 사용시 화살이 줄어드는 내용이 필요함
                for(int i = 0; i < how_many; i++)
                {
                    GameObject a = Instantiate(skill_Arrow);
                    a.transform.position = pos_s;
                    a.transform.forward = -player.transform.forward;
                    a.transform.Rotate(0, 10f * (i - how_many/2f),0);
                    temp.Add(a);
                }
                float arrow_moved = 0f;
                float time = 0f;
                float speed = 15f;
                while(arrow_moved < distance && time < 5f)
                {
                    float move_distance = Time.deltaTime * speed;
                    foreach(GameObject i in temp)
                    {
                        i.transform.position += (-i.transform.forward * move_distance);
                    }
                    arrow_moved += move_distance;
                    time += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                for(int i = 0; i < enemies.Count; i++)
                {
                    Vector3 targetDir = (enemies[i].transform.position - player.transform.position).normalized;//방향 계산

                    float dot = Vector3.Dot(player.transform.forward, targetDir);//내적 계산

                    float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;//내적과 acos을 이용하여 사이각 계산

                    float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

                    if(theta <= SightAngle && far < distance)
                        enemies[i].GetComponent<Stat>().OnAttacked(damage, my_stat);
                }
                foreach(GameObject i in temp)
                    Destroy(i);
                cool[2] = cool_max[2];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }

    IEnumerator Archer_R()
    {
        float damage = my_stat.Attack * 5f;
        pos_selected = false;
        canceled = false;
        Vector3 pos;
        StartCoroutine(Show_ArrowIndicator(true,5));//range값은 5넘어가면 안됨
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    destination.Clear();
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        player.transform.forward = (new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized);
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.Q))
                {
                    canceled = true;
                    on_skill = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if(pos_selected)
            {
                curState = State.STATE_SKILL;
                skill = HotKey.R;
                Ani_State_Change();
                yield return new WaitForSeconds(3.6f);
                GameObject temp = Instantiate<GameObject>(R_Arrow);
                temp.transform.position = player.transform.position;
                Vector3 dir = player.transform.forward;
                temp.GetComponent<FireArrow>().Run(my_stat,damage, dir);
                cool[3] = cool_max[3];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }

    IEnumerator Archer_Passive()
    {
        while(true)
        {
            if(my_stat.level_up)
            {
                attackRange = 15 + (my_stat.Level-1) * 0.1f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Archer : Player
{
    GameObject Arrow;
    public override void abstract_Init()
    {
        job = "Archer";
        Arrow = Resources.Load<GameObject>("Prefabs/Arrow");
        
        attackRange = 15f;
        cool_max[0] = 1f;
        cool_max[1] = 1f;
        cool_max[2] = 1f;
        cool_max[3] = 1f;
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
        }
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
        Vector3 pos = player.transform.GetChild(2).transform.GetChild(0).position;//화살이 활에서 나가도록 조정해주어야함
        if(!on_skill)//스킬을 사용중이라면 공격할 수 없음
        {
            isAttack = true;
            isMove = false;

            curState = State.STATE_ATTACK;
            //Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
            Ani_State_Change();
            player.transform.right = -(my_enemy.transform.position - player.transform.position).normalized;
            ani.SetFloat("AttackSpeed",attack_speed);
            yield return new WaitForSeconds(3.8f/attack_speed);//공격 애니메이션 시간
            
            if(my_enemy != null)
            {
                GameObject temp = GameObject.Instantiate<GameObject>(Arrow);
                temp.transform.position = pos;
                    
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
        StartCoroutine(Archer_W(0.5f,5f,4f));
    }

    public override void E()
    {
        StartCoroutine(Archer_E(60f,attackRange,15f));
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

    IEnumerator Archer_W(float speed, float range, float time)
    {
        attackRange += range;
        my_stat.AttackSpeed += speed;
        on_skill = false;
        cool[0] = cool_max[0];//시전시간이 없어서 일단은 바로 쿨 돌리기
        yield return new WaitForSeconds(time);//지속시간
        attackRange -= range;
        my_stat.AttackSpeed -= speed;
    }

    IEnumerator Archer_E(float SightAngle, float distance,float damage)
    {
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
                        player.transform.right = -new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized;
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
                Vector3 pos_s = player.transform.GetChild(2).transform.GetChild(0).position;
                yield return new WaitForSeconds(1.9f);
                List<GameObject> temp = new List<GameObject>();
                int how_many = (int)(SightAngle/10f);
    
                GameObject skill_arrow = Resources.Load<GameObject>("Prefabs/Skill_Arrow_small_size");
                for(int i = 0; i < how_many; i++)
                {
                    GameObject a = Instantiate(skill_arrow);
                    a.transform.position = pos_s;
                    a.transform.forward = -player.transform.right;
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
                        i.transform.position += (i.transform.forward * move_distance);
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
                        enemies[i].GetComponent<Stat>().Hp -= damage;
                }
                foreach(GameObject i in temp)
                    Destroy(i);
                cool[0] = cool_max[0];
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
                        player.transform.right = -(new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized);
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
                yield return new WaitForSeconds(3.8f);
                GameObject temp = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Skill_Arrow"));
                temp.transform.position = player.transform.position;
                Vector3 dir = -player.transform.right;
                temp.GetComponent<FireArrow>().Run(my_stat,100,dir);
                cool[0] = cool_max[0];
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
                attackRange = 15 + (my_stat.Level-1) * 0.25f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}

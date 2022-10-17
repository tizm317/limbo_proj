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
        if(GameObject.Find("Indicator") == null)
            Indicator = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ArrowIndicator_modified"));
        Indicator.name = "Indicator";
        Indicator.SetActive(false);
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
        Vector3 pos = player.transform.GetChild(1).transform.GetChild(0).position;//화살이 활에서 나가도록 조정해주어야함
        if(!on_skill)//스킬을 사용중이라면 공격할 수 없음
        {
            isAttack = true;
            isMove = false;

            curState = State.STATE_ATTACK;
            //Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
            Ani_State_Change();
            player.transform.right = -(my_enemy.transform.position - player.transform.position).normalized;
            ani.SetFloat("AttackSpeed",attack_speed);
            yield return new WaitForSeconds((5f - 0.6f)/attack_speed);//공격 애니메이션 시간
            
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
        
    }

    public override void R()
    {
        StartCoroutine(Archer_R());
    }

    public override void Passive()
    {
        
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
                        Debug.Log(player.transform.forward);
                        Debug.Log(new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized);
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
                yield return new WaitForSeconds(1.633f);
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

    IEnumerator Archer_E()
    {
        yield return new WaitForEndOfFrame();
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
                yield return new WaitForSeconds(4.4f);
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
}

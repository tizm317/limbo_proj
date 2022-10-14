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
        cool_max[0] = 5f;
        cool_max[1] = 4f;
        cool_max[2] = 3f;
        cool_max[3] = 15f;
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
        
    }

    public override void W()
    {
        
    }

    public override void E()
    {
        
    }

    public override void R()
    {
        
    }

    public override void Passive()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Warrior : Player
{
    public override void abstract_Init()
    {
        //클래스, 사거리, 스킬 쿨타임 초기화 지정
        if(GameObject.Find("Indicator") == null)
            Indicator = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/CircleIndicator_modified"));
        Indicator.name = "Indicator";
        Indicator.SetActive(false);
        job = "Warrior";
        attackRange = 3f;
        cool_max[0] = 1f;
        cool_max[1] = 1f;
        cool_max[2] = 1f;
        cool_max[3] = 1f;
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
        }
    }

    public override void Q()
    {
        StartCoroutine(Warrior_Q(45f,5f,15f));
    }

    public override void W()
    {
        StartCoroutine(Warrior_W(2f,5f,5f));  
    }

    public override void E()
    {
        StartCoroutine(Warrior_E(5f, 50));
    }

    public override void R()
    {
        StartCoroutine(Warrior_R(2f,5f));
    }

    public override void Passive()
    {
        StartCoroutine(Warrior_Passive(1f));
    }

    IEnumerator Warrior_Q(float SightAngle, float distance,float damage)
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
            if(pos_selected)
            {
                curState = State.STATE_SKILL;
                skill = HotKey.Q;
                Ani_State_Change();
                yield return new WaitForSeconds(2.2f);

                for(int i = 0; i < enemies.Count; i++)
                {
                    Vector3 targetDir = (enemies[i].transform.position - player.transform.position).normalized;//방향 계산

                    float dot = Vector3.Dot(player.transform.forward, targetDir);//내적 계산

                    float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;//내적과 acos을 이용하여 사이각 계산

                    float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

                    if(theta <= SightAngle && far < distance)
                        enemies[i].GetComponent<Stat>().Hp -= damage;
                }
                cool[0] = cool_max[0];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }

    IEnumerator Warrior_W(float mul, float time, float range)
    {
        //mul = 체젠 배율
        //time = 얼마나 오랫동안?
        curState = State.STATE_SKILL;
        skill = HotKey.W;
        Ani_State_Change();
        yield return new WaitForSeconds(2.5f);//애니메이션을 시전하는 동안

        my_stat.Regeneration *= mul;
        cool[1] = cool_max[1];
        on_skill = false;

        //도발에 대한 내용이 있어야함 - 희진누나랑 상담해볼 부분
        for(int i = 0; i < enemies.Count; i++)
                {
                    float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

                    if(far < range)
                        enemies[i].GetComponent<Enemy>().set_target(player);
                }

        yield return new WaitForSeconds(time);
        my_stat.Regeneration /= mul;
        
        
    }
    IEnumerator Warrior_E(float range, float percent)
    {
        curState = State.STATE_SKILL;
        skill = HotKey.E;
        Ani_State_Change();
        yield return new WaitForSeconds(1f);
        Managers.Sound.Play("Sound/warrior_yelling",Define.Sound.Effect, 1.0f, true);
        yield return new WaitForSeconds(4f);
        //적의 공격속도와 공격력을 감소시키는 내용이 있어야함 - 희진누나랑 상담해볼 부분
        cool[2] = cool_max[2];
        on_skill = false;
        List<Stat> temp = new List<Stat>();
        for(int i = 0; i < enemies.Count; i++)
        {
            float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

            if(far < range)
            {
                enemies[i].GetComponent<Stat>().Attack *= (percent / 100f);
                enemies[i].GetComponent<Stat>().AttackSpeed *= (percent / 100f);
                temp.Add(enemies[i].GetComponent<Stat>());
            }
        }

        yield return new WaitForSeconds(5f);//이 시간만큼 공속과 이속을 깍아준 뒤

        foreach(Stat i in temp)
        {
            i.Attack /= (percent / 100f);
            i.AttackSpeed /= (percent / 100f);
        }//원상복귀
    }

    IEnumerator Warrior_R(float range, float damage)
    {
        canceled = false;
        pos_selected = false;
        Vector3 pos;
        StartCoroutine(Show_CircleIndicator(false,360,range));
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
                yield return new WaitForSeconds(1.8f);
                StartCoroutine(CameraShake(0.5f));
                yield return new WaitForSeconds(0.4f);
                for(int i = 0; i < enemies.Count; i++)
                {
                    float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

                    if(far < range)
                        enemies[i].GetComponent<Stat>().Hp -= damage;
                }
                cool[3] = cool_max[3];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }
    IEnumerator Warrior_Passive(float percent)
    {
        //percent는 잃은 체력의 몇 퍼센트 인지를 나타냄
        //이 함수는 초기에 한번만 돌려주면 됨
        while(true)
        {
            yield return new WaitForSeconds(1f);//1초마다 반복해서 수행
            if(my_stat.Hp < my_stat.MaxHp && curState != State.STATE_DIE)//죽지 않고 최대 체력보다 체력이 적다면 패시브 동작
            {
                my_stat.Hp += (my_stat.MaxHp - my_stat.Hp)*percent/100f;
                if(my_stat.Hp > my_stat.MaxHp)
                    my_stat.Hp = my_stat.MaxHp;//패시브 돌렸는데 체력이 최대 체력보다 큰 경우는 그냥 최대체력으로 정의
            }
            else if(curState == State.STATE_DIE)//만약 죽었다면 탈출(캐릭터를 삭제하고 새로 생성한다면 필요함, 그게 아니라면 지워도 됨)
                break;
        }
    }

#region 카메라 쉐이크

    IEnumerator CameraShake(float duration)
    {
        Vector3 original_position = cam.transform.position;
        cam.GetComponent<Camera_Controller>().camera_control = false;
        float time = 0f;
        float range = 0.1f;
        while(time < duration)
        {
            Vector3 new_pos = original_position;
            if(Camera.main.transform.position != original_position)
                new_pos = original_position;
            else
                new_pos = original_position +new Vector3((Random.Range(-1f,1)>0)?range:-range,(Random.Range(-1f,1)>0)?range:-range,0);
            Camera.main.transform.position = new_pos;
            yield return new WaitForSeconds(0.1f);
            time += 0.05f;           
        }
        
        if(Camera.main.transform.position != original_position)
        {
            cam.transform.position = original_position;
        }
        cam.GetComponent<Camera_Controller>().camera_control = true;
    }

#endregion
}

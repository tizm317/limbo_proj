using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Warrior : Player
{

    public Buff _buff;
    GameObject Debuff_Effect, Explosion;

    public override void abstract_Init()
    {
        //클래스, 사거리, 스킬 쿨타임 초기화 지정
        //job = "Warrior";
        attackRange = 3f;
        
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
        }
        _buff = gameObject.AddComponent<Buff>();
        Debuff_Effect = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Swamp");
        Explosion = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Explode10");
    }

    public override void Cool_Update()//스킬 레벨업 할때 한번씩 돌아가면됨
    {
       cool_max[0] = 10f - (2f * skill_level[0] -1);
       cool_max[1] = 10f - (skill_level[1] - 1);
       cool_max[2] = 20f - (2f * skill_level[2] -1);
       cool_max[3] = 30 - 4f * skill_level[3];
    }

    public override void Q()
    {
        StartCoroutine(Warrior_Q());
    }

    public override void W()
    {
        StartCoroutine(Warrior_W());  
    }

    public override void E()
    {
        StartCoroutine(Warrior_E());
    }

    public override void R()
    {        
        StartCoroutine(Warrior_R());
    }

    public override void Passive()
    {
        StartCoroutine(Warrior_Passive());
    }

    IEnumerator Warrior_Q()
    {
        float SightAngle = 45f;
        float distance = attackRange * 1.5f;
        float damage = my_stat.Attack * (1 + skill_level[0] * 0.25f);

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
                curState = State.Skill;
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
                    {
                        enemies[i].GetComponent<Stat>().OnAttacked(damage,my_stat);
                    }
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

    IEnumerator Warrior_W()
    {
        //mul = 체젠 배율
        //time = 얼마나 오랫동안?
        float mul = 1 + skill_level[1];
        float time = 5f;
        float range = attackRange * (2 + skill_level[1]);
        curState = State.Skill;
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
                    {
                        enemies[i].GetComponent<Enemy>().set_target(player);
                    }
                }

        yield return new WaitForSeconds(time);
        my_stat.Regeneration /= mul;
        
        
    }
    IEnumerator Warrior_E()
    {
        float range = attackRange * (2 + skill_level[2]);
        float percent = 5f * (1 + skill_level[2]);
        float time = 5f;
        curState = State.Skill;
        skill = HotKey.E;
        Ani_State_Change();
        yield return new WaitForSeconds(1f);
        Managers.Sound.Play("Sound/warrior_yelling",Define.Sound.Effect, 1.0f, true);
        yield return new WaitForSeconds(4f);
        //적의 공격속도와 공격력을 감소시키는 내용이 있어야함 - 희진누나랑 상담해볼 부분
        cool[2] = cool_max[2];
        on_skill = false;
        List<Stat> temp = new List<Stat>();
        List<GameObject> temp2 = new List<GameObject>();
        
        for(int i = 0; i < enemies.Count; i++)
        {
            float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

            if(far < range)
            {
                enemies[i].GetComponent<Stat>().Attack *= (percent / 100f);
                enemies[i].GetComponent<Stat>().AttackSpeed *= (percent / 100f);
                temp.Add(enemies[i].GetComponent<Stat>());
                GameObject eff = Instantiate<GameObject>(Debuff_Effect);
                eff.transform.localScale = Vector3.zero;
                eff.transform.SetParent(enemies[i].transform);
                eff.transform.localPosition = Vector3.zero;
                temp2.Add(eff);
            }
        }
        float wanted_size = 0.1f;
        float cur_size = 0f;
        while(Mathf.Abs(wanted_size - cur_size) > 0.01f)
        {
            cur_size = Mathf.Lerp(cur_size,wanted_size,Time.deltaTime * 2f);
            foreach(GameObject i in temp2)
            {
                i.transform.localScale = Vector3.one * cur_size;
            }
        }
        

        yield return new WaitForSeconds(time);//이 시간만큼 공속과 이속을 깍아준 뒤

        foreach(Stat i in temp)
        {
            i.Attack /= (percent / 100f);
            i.AttackSpeed /= (percent / 100f);
        }//원상복귀
        wanted_size = 0f;
        cur_size = 0.1f;
        while(Mathf.Abs(wanted_size - cur_size) > 0.01f)
        {
            cur_size = Mathf.Lerp(cur_size,wanted_size,Time.deltaTime * 2f);
            foreach(GameObject i in temp2)
            {
                i.transform.localScale = Vector3.one * cur_size;
            }
        }
        foreach(GameObject i in temp2)
            Destroy(i);
    }
    IEnumerator Warrior_R()
    {
        float range = 1 + skill_level[3];
        float damage = my_stat.Attack * (skill_level[3] * 0.5f + 1f);
        canceled = false;
        pos_selected = false;
        GameObject temp= Instantiate<GameObject>(Explosion);
        temp.SetActive(false);
        Vector3 pos = player.transform.position;
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
                if(Vector3.Distance(player.transform.position, pos) > 3.5f)
                {
                    curState = State.Move;
                    Ani_State_Change();
                    Set_Destination(pos);
                    while(Vector3.Distance(player.transform.position, pos) > 3.5f)
                    {
                        if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.Q)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E))
                        {
                            canceled = true;
                            on_skill = false;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    destination.Clear();
                }
                curState = State.Skill;
                skill = HotKey.R;
                Ani_State_Change();
                yield return new WaitForSeconds(1.8f);
                StartCoroutine(CameraShake(0.5f));
                temp.SetActive(true);
                temp.transform.position = player.transform.position;
                temp.transform.localScale = Vector3.one * 0.1f * range;
                yield return new WaitForSeconds(0.4f);
                for(int i = 0; i < enemies.Count; i++)
                {
                    if(enemies[i] != null)
                    {
                        float far = Vector3.Distance(player.transform.position, enemies[i].transform.position);

                        if(far < range)
                            enemies[i].GetComponent<Stat>().OnAttacked(damage,my_stat);
                    }
                }
                
                cool[3] = cool_max[3];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
        yield return new WaitForSeconds(5f);
        Destroy(temp);
    }
    IEnumerator Warrior_Passive()
    {
        //percent는 잃은 체력의 몇 퍼센트 인지를 나타냄
        //이 함수는 초기에 한번만 돌려주면 됨
        float percent = 1f;
        while(true)
        {
            yield return new WaitForSeconds(1f);//1초마다 반복해서 수행
            if(my_stat.Hp < my_stat.MaxHp && curState != State.Die)//죽지 않고 최대 체력보다 체력이 적다면 패시브 동작
            {
                my_stat.Hp += (my_stat.MaxHp - my_stat.Hp)*percent/100f;
                if(my_stat.Hp > my_stat.MaxHp)
                    my_stat.Hp = my_stat.MaxHp;//패시브 돌렸는데 체력이 최대 체력보다 큰 경우는 그냥 최대체력으로 정의
            }
            else if(curState == State.Die)//만약 죽었다면 탈출(캐릭터를 삭제하고 새로 생성한다면 필요함, 그게 아니라면 지워도 됨)
                break;
        }
    }


}

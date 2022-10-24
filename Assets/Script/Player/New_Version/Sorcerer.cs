using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Player
{
    GameObject Magic_bolt;
    GameObject Icy_Field;
    [SerializeField]
    float mana_for_attack = 5f;

    public enum BlendMode
    {
        Opaque = 0,
        Cutout,
        Fade,
        Transparent
    }

    public override void abstract_Init()
    {
        job = "Sorcerer";
        Magic_bolt = Resources.Load<GameObject>("Prefabs/Magic_bolt");
        Icy_Field = Resources.Load<GameObject>("Prefabs/Icy_Field");
        attackRange = 10f;
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
                StartCoroutine(Attack(my_stat.AttackSpeed, 10f));
            }
        }
    }

    IEnumerator Attack(float attack_speed, float flight_speed)//공격 함수 구현부
    {
        if(my_stat.Mana >= mana_for_attack)
        {
            Vector3 pos = player.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).position;//화살이 활에서 나가도록 조정해주어야함
            if(!on_skill)//스킬을 사용중이라면 공격할 수 없음
            {
                isAttack = true;
                isMove = false;

                curState = State.STATE_ATTACK;
                //Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
                Ani_State_Change();
                player.transform.forward = (my_enemy.transform.position - player.transform.position).normalized;
                ani.SetFloat("AttackSpeed",attack_speed);
                yield return new WaitForSeconds(1.2f/attack_speed);//공격 애니메이션 시간
                
                if(my_enemy != null)
                {
                    GameObject temp = GameObject.Instantiate<GameObject>(Magic_bolt);
                    temp.transform.position = pos;
                        
                    temp.GetComponent<Arrow>().Arrow_(my_enemy,15f,this);
                    my_stat.Mana -= mana_for_attack;
                }
                
                curState = State.STATE_IDLE;
                Ani_State_Change();
                yield return new WaitForSeconds(1/attack_speed);//1초를 공격속도로 나눈 값만큼 기다렸다가 다음 공격을 수행
                isAttack = false;     
            }
        }
    }

    public override void Q()
    {
        StartCoroutine(Sorcerer_Q(5f));
    }

    public override void W()
    {
        StartCoroutine(Sorcerer_W(50f, 15f));
    }

    public override void E()
    {
        StartCoroutine(Sorcerer_E(10f,3f,30f,0.5f));
    }

    public override void R()
    {
        
    }

    public override void Passive()
    {
        
    }
#region Q
    IEnumerator Sorcerer_Q(float time)
    {
        curState = State.STATE_SKILL;
        skill = HotKey.Q;
        Ani_State_Change();
        yield return new WaitForSeconds(3.250f);
        attackable = false;
        player.tag = "Untagged";
        changeRenderMode(player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial,BlendMode.Transparent);
        float wanted_alpha = 0.2f;
        float a = 0;
        while(true)
        {
            a += Time.deltaTime;
            Color body_Color = player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetColor("_Color");
            float my_alpha = body_Color.a;
            my_alpha = Mathf.Lerp(my_alpha,wanted_alpha,0.1f);
            player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.SetColor("_Color",new Color(1,1,1,my_alpha));
            if(Mathf.Abs(my_alpha - wanted_alpha) < 0.01)
                break;
            yield return new WaitForEndOfFrame();
        }
        cool[0] = cool_max[0];
        on_skill = false;
        yield return new WaitForSeconds(time);
        wanted_alpha = 1f;
        while(true)
        {
            a += Time.deltaTime;
            Color body_Color = player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetColor("_Color");
            float my_alpha = body_Color.a;
            my_alpha = Mathf.Lerp(my_alpha,wanted_alpha,0.1f);
            player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.SetColor("_Color",new Color(1,1,1,my_alpha));

            if(Mathf.Abs(my_alpha - wanted_alpha) < 0.01)
                break;
            yield return new WaitForEndOfFrame();
        }
        changeRenderMode(player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial,BlendMode.Opaque);
        attackable = true;
        player.tag = "Player";
        player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.SetColor("_Color",new Color(1,1,1,1f));
    }

    public static void changeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetFloat("_Mode",0.0f);
                standardShaderMaterial.SetOverrideTag("RenderType", "Opaque");
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetFloat("_Mode",1.0f);
                standardShaderMaterial.SetOverrideTag("RenderType", "Opaque");
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetFloat("_Mode",2.0f);
                standardShaderMaterial.SetOverrideTag("RenderType", "Transparent");
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetFloat("_Mode",3.0f);
                standardShaderMaterial.SetOverrideTag("RenderType", "Transparent");
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }
    }
#endregion

#region W
    IEnumerator Sorcerer_W(float heal, float range)
    {
        pos_selected = false;
        canceled = false;
        Vector3 pos;
        GameObject target = null;
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
                    else if(hit.collider.tag == "Player")
                    {
                        target = hit.transform.gameObject;
                        pos_selected = true;
                        pos = target.transform.position;
                        player.transform.forward = new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized;
                    }
                    else
                        canceled = true;
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.Q)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.R))
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
                if(Vector3.Distance(player.transform.position, target.transform.position) > range)
                {
                    while(Vector3.Distance(player.transform.position, target.transform.position) > range)
                    {
                        curState = State.STATE_MOVE;
                        Ani_State_Change();
                        Set_Destination(target.transform.position);
                        if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.Q)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.R))
                        {
                            canceled = true;
                            on_skill = false;
                            break;
                        }
                    }
                    destination.Clear();
                }
                if(Vector3.Distance(player.transform.position, target.transform.position) <= range)
                {
                    curState = State.STATE_SKILL;
                    skill = HotKey.W;
                    Ani_State_Change();
                    yield return new WaitForSeconds(2.6f);
                    target.GetComponent<PlayerStat>().Hp += heal;
                    if(target.GetComponent<PlayerStat>().Hp>target.GetComponent<PlayerStat>().MaxHp)
                        target.GetComponent<PlayerStat>().Hp = target.GetComponent<PlayerStat>().MaxHp;
                    cool[0] = cool_max[0];
                    on_skill = false;
                    break;
                }    
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }
#endregion

#region E
    IEnumerator Sorcerer_E(float range, float width, float damage, float slow)
    {
        canceled = false;
        pos_selected = false;
        Vector3 pos = player.transform.position;
        Vector3 wanted_size;
        float wanted_height;
        float generate_speed = 0.05f;
        List<Stat> slowed_enemy = new List<Stat>();
        GameObject temp = Instantiate(Icy_Field);
        GameObject sword = temp.transform.GetChild(3).gameObject;
        temp.SetActive(false);
        temp.transform.localScale = Vector3.zero;
        StartCoroutine(Show_CircleIndicator(false,360,width));
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
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.R)||Input.GetKey(KeyCode.Q))
                {
                    canceled = true;
                    on_skill = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if(pos_selected)
            {
                if(Vector3.Distance(player.transform.position, pos) > range)
                {
                    curState = State.STATE_MOVE;
                    Ani_State_Change();
                    Set_Destination(pos);
                    while(Vector3.Distance(player.transform.position, pos) > range)
                    {
                        if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.Q)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.R))
                        {
                            canceled = true;
                            on_skill = false;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    destination.Clear();
                }
                curState = State.STATE_SKILL;
                skill = HotKey.E;
                Ani_State_Change();
                yield return new WaitForSeconds(0.7f);
                temp.transform.position = pos;
                temp.SetActive(true);
                wanted_size = new Vector3(1f,1f,5/9f) * width;
                
                while(true)
                {
                    Vector3 temp_size = temp.transform.localScale;

                    if(Mathf.Abs(wanted_size.x - temp_size.x) <= 0.01f)
                        break;
                    float x = Mathf.Lerp(temp_size.x,wanted_size.x,generate_speed);
                    float y = Mathf.Lerp(temp_size.y,wanted_size.y,generate_speed);
                    float z = Mathf.Lerp(temp_size.z,wanted_size.z,generate_speed);
                    temp.transform.localScale = new Vector3(x,y,z);
                    yield return new WaitForEndOfFrame();
                }
                wanted_height = 0.5f;
                while(true)
                {
                    if(Mathf.Abs(wanted_height - sword.transform.localPosition.z) <= 0.1f)
                        break;
                    float height = Mathf.Lerp(sword.transform.localPosition.z,wanted_height,0.1f);
                    Debug.Log(height);
                    sword.transform.localPosition = new Vector3(0,0,height);
                    yield return new WaitForEndOfFrame();
                }
                for(int i = 0; i < enemies.Count; i++)
                {
                    if(enemies[i] != null)
                    {
                        float far = Vector3.Distance(pos, enemies[i].transform.position);

                        if(far < width)
                        {
                            enemies[i].GetComponent<Stat>().OnAttacked(damage,my_stat);
                            enemies[i].GetComponent<Stat>().MoveSpeed *= slow;
                            slowed_enemy.Add(enemies[i].GetComponent<Stat>());
                        } 
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
        yield return new WaitForSeconds(1f);//필드 사라지는 시간
        wanted_size = new Vector3(0f,0f,0f);
        wanted_height = -2.5f;
        while(true)
        {
            if(Mathf.Abs(wanted_height - sword.transform.localPosition.z) <= 0.1f)
                break;
            float height = Mathf.Lerp(sword.transform.localPosition.z,wanted_height,0.1f);
            sword.transform.localPosition = new Vector3(0,0,height);
            yield return new WaitForEndOfFrame();
        }
        while(true)
        {
            Vector3 temp_size = temp.transform.localScale;
            if(Mathf.Abs(wanted_size.x - temp_size.x) <= 0.01f)
                break;
            float x = Mathf.Lerp(temp_size.x,wanted_size.x,generate_speed);
            float y = Mathf.Lerp(temp_size.y,wanted_size.y,generate_speed);
            float z = Mathf.Lerp(temp_size.z,wanted_size.z,generate_speed);
            temp.transform.localScale = new Vector3(x,y,z);
            yield return new WaitForEndOfFrame();
        }
        Destroy(temp);//최초 1회만 생성하고 반복해서 사용하게 수정할 수 있음
        yield return new WaitForSeconds(2f);//적들 이동속도 회복 시간
        foreach(Stat i in slowed_enemy)
            i.MoveSpeed /= slow;
    }
#endregion
}

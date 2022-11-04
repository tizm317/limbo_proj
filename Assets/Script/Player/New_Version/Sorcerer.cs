using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Sorcerer : Player
{
    GameObject Magic_bolt;
    GameObject Icy_Field,Icy_Smoke;
    GameObject Smoke;
    GameObject Shield;
    GameObject Heal_effect;
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
        Magic_bolt = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/BlueBall");
        Icy_Field = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Icy_Field");
        Shield = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/skull shield");
        Icy_Smoke = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/IceCloud");
        Smoke = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/Kunai5");
        Heal_effect = Resources.Load<GameObject>("Prefabs/Prejectiles&Effects/SummonMagicCircle2");
        attackRange = 10f;
        
        for(int i = 0; i < cool.Length; i++)
        {
            cool[i] = 0;
        }
    }

    public override void Cool_Update()
    {
       cool_max[0] = 10f - (skill_level[0] -1);
       cool_max[1] = 20f - (3f * skill_level[1] - 1);
       cool_max[2] = 10f - (2f * skill_level[2] -1);
       cool_max[3] = 30 - 4f * skill_level[3];
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
                yield return new WaitForSeconds(2/attack_speed);//1초를 공격속도로 나눈 값만큼 기다렸다가 다음 공격을 수행
                isAttack = false;     
            }
        }
    }

    public override void Q()
    {
        
        StartCoroutine(Sorcerer_Q());
    }

    public override void W()
    {
        StartCoroutine(Sorcerer_W());
    }

    public override void E()
    {
        StartCoroutine(Sorcerer_E());
    }

    public override void R()
    {
        StartCoroutine(Sorcerer_R());
    }

    public override void Passive()
    {
        
    }
#region Q
    IEnumerator Sorcerer_Q()
    {
        float time = 4f + skill_level[0];
        curState = State.STATE_SKILL;
        skill = HotKey.Q;
        Ani_State_Change();
        yield return new WaitForSeconds(3.250f);
        attackable = false;
        player.tag = "Untagged";
        changeRenderMode(player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial,BlendMode.Transparent);
        GameObject temp = Instantiate<GameObject>(Smoke);
        temp.transform.SetParent(player.transform);
        temp.transform.localPosition = Vector3.zero;
        float wanted_alpha = 0.2f;
        float a = 0;
        while(true)
        {
            a += Time.deltaTime;
            Color body_Color = player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetColor("_Color");
            float my_alpha = body_Color.a;
            my_alpha = Mathf.Lerp(my_alpha,wanted_alpha,0.1f);
            temp.transform.localScale = new Vector3((1f-my_alpha)*0.1f,(1f-my_alpha)*0.1f,(1f-my_alpha)*0.1f);
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
            temp.transform.localScale = new Vector3((1f-my_alpha)*0.1f,(1f-my_alpha)*0.1f,(1f-my_alpha)*0.1f);
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
    IEnumerator Sorcerer_W()
    {
        Debug.Log("@");
        float heal = my_stat.Attack;
        float range = attackRange;
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
                    cool[1] = cool_max[1];
                    on_skill = false;
                    target.GetComponent<PlayerStat>().Hp += heal;
                    GameObject temp = Instantiate<GameObject>(Heal_effect);
                    temp.transform.SetParent(target.transform);
                    temp.transform.localPosition = Vector3.zero;
                    float wanted_size = 0.1f;
                    float effect_size = 0f;
                    float speed = 3f;
                    while(Mathf.Abs(wanted_size-effect_size) > 0.01f)
                    {
                        effect_size = Mathf.Lerp(effect_size,wanted_size,Time.deltaTime*speed);
                        temp.transform.localScale = Vector3.one * effect_size;
                    }
                    if(target.GetComponent<PlayerStat>().Hp>target.GetComponent<PlayerStat>().MaxHp)
                        target.GetComponent<PlayerStat>().Hp = target.GetComponent<PlayerStat>().MaxHp;
                    wanted_size = 0f;
                    while(Mathf.Abs(wanted_size-effect_size) > 0.01f)
                    {
                        effect_size = Mathf.Lerp(effect_size,wanted_size,Time.deltaTime*speed);
                        temp.transform.localScale = Vector3.one * effect_size;
                    }
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
    IEnumerator Sorcerer_E()
    {
        float range = attackRange;
        float width = skill_level[2] + 1;
        float damage = my_stat.Attack * (1 + skill_level[2] * 0.25f);
        float slow = 20f + skill_level[2] * 5f;
        float time = 5f;
        canceled = false;
        pos_selected = false;
        Vector3 pos = player.transform.position;
        Vector3 wanted_size;
        float size, wanted_size2;
        float wanted_height;
        float generate_speed = 0.05f;
        List<GameObject> slowed_enemy = new List<GameObject>();
        GameObject temp = Instantiate<GameObject>(Icy_Field);
        GameObject temp2 = Instantiate<GameObject>(Icy_Smoke);
        GameObject sword = temp.transform.GetChild(3).gameObject;
        temp.SetActive(false);
        temp.transform.localScale = Vector3.zero;
        temp2.SetActive(false);
        temp2.transform.localScale = Vector3.zero;
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
                temp2.transform.position = pos;
                temp.SetActive(true);
                temp2.SetActive(true);
                wanted_size = new Vector3(1f,1f,5/9f) * width;
                wanted_size2 = 0.1f * width;
                size = 0f;
                while(true)
                {
                    Vector3 temp_size = temp.transform.localScale;

                    if(Mathf.Abs(wanted_size.x - temp_size.x) <= 0.01f)
                        break;
                    float x = Mathf.Lerp(temp_size.x,wanted_size.x,generate_speed);
                    float y = Mathf.Lerp(temp_size.y,wanted_size.y,generate_speed);
                    float z = Mathf.Lerp(temp_size.z,wanted_size.z,generate_speed);
                    size = Mathf.Lerp(size,wanted_size2,generate_speed);
                    temp.transform.localScale = new Vector3(x,y,z);
                    temp2.transform.localScale = Vector3.one * size;
                    yield return new WaitForEndOfFrame();
                }
                wanted_height = 0.5f;
                while(true)
                {
                    if(Mathf.Abs(wanted_height - sword.transform.localPosition.z) <= 0.1f)
                        break;
                    float height = Mathf.Lerp(sword.transform.localPosition.z,wanted_height,0.1f);
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
                            if(enemies[i].GetComponent<Stat>().Hp <= 0)
                                break;
                            enemies[i].GetComponent<NavMeshAgent>().speed *= (slow / 100f);
                            slowed_enemy.Add(enemies[i]);
                        } 
                    }
                }
                cool[2] = cool_max[2];
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
        wanted_size2 = 0f;
        size = temp2.transform.localScale.x;
        while(true)
        {
            Vector3 temp_size = temp.transform.localScale;
            if(Mathf.Abs(wanted_size.x - temp_size.x) <= 0.01f)
                break;
            float x = Mathf.Lerp(temp_size.x,wanted_size.x,generate_speed);
            float y = Mathf.Lerp(temp_size.y,wanted_size.y,generate_speed);
            float z = Mathf.Lerp(temp_size.z,wanted_size.z,generate_speed);
            temp.transform.localScale = new Vector3(x,y,z);
            size = Mathf.Lerp(size,wanted_size2,generate_speed);
            temp.transform.localScale = new Vector3(x,y,z);
            temp2.transform.localScale = Vector3.one * size;
            yield return new WaitForEndOfFrame();
        }
        Destroy(temp);//최초 1회만 생성하고 반복해서 사용하게 수정할 수 있음
        yield return new WaitForSeconds(time);//적들 이동속도 회복 시간
        foreach(GameObject i in slowed_enemy)
            i.GetComponent<NavMeshAgent>().speed = i.GetComponent<Stat>().MoveSpeed;
    }
#endregion

#region R
    IEnumerator Sorcerer_R()
    {
        float range = attackRange;
        float width = 1 + skill_level[3];
        float width2 = width + 3;
        float damage = my_stat.Attack * (1f + skill_level[3] * 0.5f);
        canceled = false;
        pos_selected = false;
        Vector3 pos = player.transform.position;
        StartCoroutine(Show_CircleIndicator(false,360,width2));
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
                if(Vector3.Distance(player.transform.position, pos) > range)
                {
                    curState = State.STATE_MOVE;
                    Ani_State_Change();
                    Set_Destination(pos);
                    while(Vector3.Distance(player.transform.position, pos) > range)
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
                curState = State.STATE_SKILL;
                skill = HotKey.R;
                Ani_State_Change();
                yield return new WaitForSeconds(0.7f);
                int how_many = (int)(width * (16f/5f));
                float rad = 360f/(how_many * 2);
                List<GameObject> shields = new List<GameObject>();
                for(int i = 0; i < how_many; i++)
                {
                    GameObject a = Instantiate(Shield);
                    a.transform.position = pos - new Vector3(0,3f,0);
                    a.transform.Rotate(0, (rad * (i+1)),0);
                    a.transform.Translate(a.transform.forward * width2);
                    a.transform.LookAt(new Vector3(pos.x,a.transform.position.y,pos.z));
                    shields.Add(a);
                }

                float wanted_height = 1f;
                while(true)//위로 튀어나오는 부분
                {
                    float height = 0f;;
                    foreach(GameObject i in shields)
                    {
                        height = Mathf.Lerp(i.transform.position.y, wanted_height, 2f * Time.deltaTime);
                        i.transform.position = new Vector3(i.transform.position.x, height,i.transform.position.z);
                    }
                    
                    if(Mathf.Abs(wanted_height-height) <= 0.01f)
                        break;
                    yield return new WaitForEndOfFrame();
                }
                foreach(GameObject i in shields)
                {
                    i.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = false;
                }
                cool[3] = cool_max[3];
                on_skill = false;
                float wanted_move = width2 - width;
                float move = 0f, move_before = 0f;
                while(true)//앞으로 이동
                {
                    move = Mathf.Lerp(move, wanted_move, 2f * Time.deltaTime);
                    foreach(GameObject i in shields)
                    {
                        i.transform.localPosition += i.transform.forward * (move - move_before);
                    }
                    move_before = move;
                    if(Mathf.Abs(wanted_move-move) <= 0.01f)
                        break;
                    yield return new WaitForEndOfFrame();
                }
                Vector3 wanted_size = new Vector3(2f,2f,width);
                float size = 0;
                foreach(GameObject i in shields)//광선 on
                {
                    i.transform.GetChild(1).gameObject.SetActive(true);
                }
                while(true)//광선 발사
                {
                    size = Mathf.Lerp(size,1,Time.deltaTime);
                    foreach(GameObject i in shields)
                    {
                        i.transform.GetChild(1).localScale = wanted_size * size;
                    }
                    
                    if(1-size <= 0.01f)
                        break;
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
                        } 
                    }
                }
                var ps = FindObjectsOfType<PlayerStat>();
                foreach(PlayerStat i in ps)
                {
                    float far = Vector3.Distance(pos, i.transform.position);

                    if(far < width2)
                    {
                        i.Hp += damage;
                    }
                }
                size = 1;
                while(true)//광선 사라져!
                {
                    size = Mathf.Lerp(size,0, 2f * Time.deltaTime);
                    foreach(GameObject i in shields)
                    {
                        i.transform.GetChild(1).localScale = wanted_size * size;
                    }
                    
                    if(size <= 0.01f)
                        break;
                    yield return new WaitForEndOfFrame();
                }
                wanted_height = -10f;
                while(true)//방패 사라져!
                {
                    float height = 0f;
                    foreach(GameObject i in shields)
                    {
                        height = Mathf.Lerp(i.transform.position.y, wanted_height, 2f * Time.deltaTime);
                        i.transform.position = new Vector3(i.transform.position.x, height,i.transform.position.z);
                    }
                    
                    if(Mathf.Abs(wanted_height-height) <= 0.01f)
                        break;
                    yield return new WaitForEndOfFrame();
                }
                foreach(GameObject i in shields)
                    Destroy(i);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }
#endregion
}

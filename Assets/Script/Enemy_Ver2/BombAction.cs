using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    Stat _stat;
    public GameObject bombEffect;  //이펙트 변수 생성

    public float destroyTime = 1.5f; //효과 제거될 시간 변수
    float currentTime = 0; //경과 시간 측정 변수

    //충돌체 감지 및 처리 함수 구현
    private void OnCollisionEnter(Collision collision)
    {
        //이펙트 프리팹 생성 Instantiate
        GameObject eff = Instantiate(bombEffect);
        //이펙트 위치 설정 (어디에 생성할 것인지)
        eff.transform.position = transform.position; //수류탄 위치와 동일하게
        //자기 오브젝트 제거
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collider collision)
    {
        Debug.Log("collider");
        if(collision.CompareTag("Player") == true)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerStat targetStat = player.GetComponent<PlayerStat>();
            _stat.Attack = 20;
            targetStat.OnAttacked(_stat);
        }
        else
        {
            if (currentTime > destroyTime)
            {
                Destroy(gameObject);
            }
            currentTime += Time.deltaTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 2f);

    }
}

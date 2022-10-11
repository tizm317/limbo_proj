using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkillTest : MonoBehaviour
{

    public GameObject firePosition; // 무기가 생성될 위치 지정
    public GameObject bombFactory; // 무기 오브젝트(프리팹)
    public float throwPower = 15.0f; //던지는 힘


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 버튼을 통해 무기 발사
        if(Input.GetMouseButtonDown(1))
        {
            GameObject bomb = Instantiate(bombFactory);
            bomb.transform.position = firePosition.transform.position;
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            //카메라의 정면 방향으로 무기에 물리적 힘을 가함
            rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);


        }
        
    }
}

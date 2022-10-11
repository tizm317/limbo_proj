﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Destroy : MonoBehaviour
{

    public float destroyTime = 1.5f; //효과 제거될 시간 변수
    float currentTime = 0; //경과 시간 측정 변수


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime > destroyTime)
        {
            Destroy(gameObject);
        }
        currentTime += Time.deltaTime;
    }
}

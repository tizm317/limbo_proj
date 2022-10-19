using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    //[SerializeField]
    //GameObject stage = null;

    // 스테이지 배열
    public GameObject[] Stages;
    // 몇 스테이지인지 저장
    [SerializeField] int stageIndex;  

    //메인 textUI
    public Text UIStage;  

    //setActive로 스테이지 변경
    //이전스테이지 SetActive(false), 현재스테이지 SetActive(true)
    public void NextsStage()
    {
        Stages[stageIndex].SetActive(false);
        stageIndex++;
        Stages[stageIndex].SetActive(true);
        //만약 stageIndex가 null이라면 게임 클리어할 수 있도록 방어 코드 작성

    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void showNextPlate()
    //{
    //    stagePlates[stepCount++].gameObject.SetActive(true);
    //}
}

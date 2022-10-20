using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    //역할 : 스테이지 관리
    //Hieralrchy 창에선 첫 스테이지만 활성화 하기 
    
    GameObject tempObj = null;

    // 스테이지 배열
    public GameObject[] Stages;
    // 몇 스테이지인지 저장
    public int stageIndex;
    // 적 배열
    public Transform[] enemys;
    // 해당 스테이지의 적의 수를 알 수 있도 몬스터의 수가 0일 떄 스테이지 종료
    public int enemyNumber = 0;

    //UI
    public Text UIStage;

    public GameObject m_PrntGo;

    [SerializeField] private GameObject[] m_ChildrenGos;

    void Init()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        playerReposition();
        m_ChildrenGos = GetChildren(m_PrntGo);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }

        return children;
    }


    //setActive로 스테이지 변경 함수
    // stageIndex에 따라 스테이지 활성화 비활성화
    //이전스테이지 SetActive(false), 현재스테이지 SetActive(true)
    public void NextsStage()
    {
        //스테이지 갯수를 확인하여 다음 스테이지로 이동/종료
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            playerReposition();

            UIStage.text = "STAGE " + (stageIndex +1);
        }
        else
        {
            //게임이 끝난 상황 game clear
            //Time.timeScale = 0; //시간을 멈춰둠
            Debug.Log("게임 클리어");
            //로딩 후 village 씬 or 경매 씬으로 활성화시켜야함
        }

        //만약 stageIndex가 null이라면 게임 클리어할 수 있도록 방어 코드 작성해야함

    }

    //플레이어의 위치 재배치 함수
    //tag를 통해 player를 찾아서 해당 위치를 재배치할 수 있도록
    void playerReposition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //player reposition
        player.transform.position = new Vector3(25.0f, 1f, 12.5f);
    }

    public void gameOver()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             Managers.Scene.LoadScene(Define.Scene.Village);
        }
    }

    //player어 죽었을 때 현재는 player 스크립트에서는 hp가 다시 차고 초기 위치로 가도록 하였지만
    //부활하지 못하도록 스크립트 추가해야함
    //hp 다운 후 0이 되면 플레이어 죽음 함수 호출 


}

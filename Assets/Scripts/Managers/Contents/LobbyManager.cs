using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    DataManager data;
    Camera[] Cameras = new Camera[3];
    [SerializeField] Image shade_img;
    [SerializeField] InputField input_name;
    float x_cam;
    float z_cam;
    float r;
    void Start()
    {
        data = new DataManager();
        if(SceneManager.GetActiveScene().name.Contains("Create"))
            Make_Character_List_Character_Create();
        else if(SceneManager.GetActiveScene().name.Contains("Lobby"))
            Make_Character_List_Lobby();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Make_Character_List_Lobby()
    {

    }

    void Make_Character_List_Character_Create()
    {
        var characters = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < Cameras.Length; i++)
        {
            Cameras[i] = characters[i].transform.GetChild(characters[i].transform.childCount -1).GetComponent<Camera>();
            Cameras[i].transform.localPosition = new Vector3(0f, 1.2f, 3f);
            Cameras[i].transform.LookAt(characters[i].transform.GetChild(0)); // Hips 기준
        }
    }

    public void Character_Select(int idx)
    {
        shade_img.gameObject.SetActive(false);
        for(int i = 0; i < Cameras.Length; i++)
        {
            if(i != idx)
            {
                Cameras[i].gameObject.SetActive(false);
            }
            else
            {
                Cameras[i].gameObject.SetActive(true);
            }
        }
        PlayerMgr.cur_JOB = (Define.Job)idx;//생성 전에 직업 변경해버리기~
    }

    public void Create_Character()
    {
        //TODO 캐릭터 이름 받은 거랑 플레이어 스텟이랑 직업 이용해서 새 캐릭터 만드는 부분 추가
        
    }

    public void Back(string scenename)
    {
        LoadingScene.LoadScene(scenename);
    }

    public void Back(Define.Scene scene)
    {
        string _scene = scene.ToString();
        LoadingScene.LoadScene(_scene);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScene : BaseScene
{
    public static string nextScene = "InGameCemetery";
    [SerializeField] Image progressBar;
    [SerializeField] Text Percent;
    // Start is called before the first frame update
    void Start()
    {
        SceneType = Define.Scene.Unknown;
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //SceneManager.LoadScene("LoadingScene");
        Managers.Scene.LoadScene(Define.Scene.LoadingScene);
    }
    public static void LoadScene(Define.Scene sceneName)
    {
        nextScene = sceneName.ToString();
        //Debug.Log(nextScene);
        //SceneManager.LoadScene("LoadingScene");
        Managers.Scene.LoadScene(Define.Scene.LoadingScene);
    }

    IEnumerator LoadScene()
    {
        Managers.Sound.Play("Sound/BGM/LoadingScene",Define.Sound.Bgm, 1.0f, true);       
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        float timer2 = 0.0f;
        while(!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
                timer2 += Time.deltaTime;
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress,timer);
                Percent.text = $"{progressBar.fillAmount * 100f}%";
                if(progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {  
                
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                Percent.text = $"{progressBar.fillAmount * 100f}%";
                if(progressBar.fillAmount == 1.0f)
                {
                    if(timer2 < 8)
                        yield return new WaitForSeconds(8f - timer2);
                    op.allowSceneActivation = true;
                    yield break;
                }

            }
        }
    }

    public override void Clear()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScene : MonoBehaviour
{
    public static string nextScene = "InGameCemetery";
    [SerializeField] Image progressBar;
    [SerializeField] Text Percent;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
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
                    yield return new WaitForSeconds(0.1f);
                    op.allowSceneActivation = true;
                    yield break;
                }

            }
        }
    }
}

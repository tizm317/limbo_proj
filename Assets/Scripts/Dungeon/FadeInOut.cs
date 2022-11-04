using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeInOut : MonoBehaviour
{
    //UIBackground
    //public Text UIStage;
    public Image UIBackground;
    public float time = 0f;
    public float F_time = 1f;
    public void Fade()
    {
        StartCoroutine(FadeFlow());
    }
    IEnumerator FadeFlow()
    {
        Color alpha = UIBackground.color;


        time = 0;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0f, 1f, time);
            UIBackground.color = alpha;
            break;
            //yield return null;
        }
        time = 0;
        yield return new WaitForSeconds(2f);
        while (alpha.a > 0)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1f, 0f, time);
            UIBackground.color = alpha;
            yield return null;
        }
        yield return null;
    }
}

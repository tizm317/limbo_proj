using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{

    private static Screenshot instance;

    private Camera GCamera;
    private bool takeScreenshot;
    //private int screenShotcount = 1;


    private void Awake()
    {
        if (instance == null || GCamera == null)
            Init();
    }

    private void Init()
    {
        instance = this;
        GCamera = Camera.main;
    }

    private void OnPostRender()
    {
        if (takeScreenshot)
        {
            takeScreenshot = false;
            RenderTexture renderTexture = GCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            string date = System.DateTime.Now.ToString().Trim().Replace('-', '_').Replace(':', '_');

            // 폴더 없으면 폴더 생성 후 저장
            bool folderExist = System.IO.Directory.Exists(Application.dataPath + "/ScreenShots/");
            if(folderExist == false)
                System.IO.Directory.CreateDirectory(Application.dataPath + "/ScreenShots/");

            string savePath =  string.Format(Application.dataPath + "/ScreenShots/Screenshot{0}.png", date);

            System.IO.File.WriteAllBytes(savePath, byteArray);
            Debug.Log($"Saved Screenshot In {savePath}");

            RenderTexture.ReleaseTemporary(renderTexture);
            GCamera.targetTexture = null;
        }
    }

    private void _takeScreenshot(int width, int height)
    {
        GCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshot = true;
    }

    public static void TakeScreenshot(int width, int height)
    {
        
        instance._takeScreenshot(width, height);
    }


}
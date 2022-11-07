using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //if (SceneManager.GetActiveScene().name == "InGame")
        //{
        //    if (collision.gameObject.name == "Player")
        //        Managers.Scene.LoadScene(Define.Scene.InGameBoss);

        //}
        //else if (SceneManager.GetActiveScene().name == "InGameNature")
        //{
        //    if (collision.gameObject.name == "Player")
        //        Managers.Scene.LoadScene(Define.Scene.InGameNatureBoss);


        //}
        //else if (SceneManager.GetActiveScene().name == "InGameDesert")
        //{
        //    if (collision.gameObject.name == "Player")
        //        Managers.Scene.LoadScene(Define.Scene.InGameDesertBoss);
        //}

    }
}

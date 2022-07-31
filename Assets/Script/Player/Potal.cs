using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //d
        Managers.Scene.LoadScene(Define.Scene.Village);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Managers.Scene.LoadScene(Define.Scene.MapTest);
        Debug.Log("test");
    }
}

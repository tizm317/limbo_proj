﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SpawningPoolBoss : MonoBehaviour
{

    [SerializeField] int _monsterCount = 0;
    int _reserveCount = 0; // 코루틴을 생성할 때 현재 예약된 코루틴이 몇개인지 판단
    [SerializeField] int _keepMonsterCount = 0;
    [SerializeField] Vector3 _spawnPos;

    float _spawnRadius = 20.0f;

    float _spawnTime = 5.0f;

    public GameObject[] enemyCharacter;
    public void AddMonsterCount(int value)
    {
        _monsterCount += value;
    }
    public void SetKeepMonsterCount(int count)
    {
        _keepMonsterCount = count;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyCharacter = Resources.LoadAll<GameObject>("Prefabs/EnemyCharacters");
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    // Update is called once per frame
    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            // 현재 몬스터의 수와 예약된 코루틴의 수가 _keepMonsterCount보다 적다면 코루틴 실행
            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));

        // _monsterCount를 이 함수에서 늘리지 않아도 GameManagerEx에서 Spawn함수가 실행될 때 Invoke로 _monsterCount를 늘려준다.
        if (SceneManager.GetActiveScene().name == "6.InGameCemetery")
        {
            enemyCharacter[0] = Managers.Game.Spawn(Define.WorldObject.Monster, "Warrok W Kurniawan");
            NavMeshAgent nma0 = enemyCharacter[0].GetComponent<NavMeshAgent>();

        }
        else if (SceneManager.GetActiveScene().name == "4.InGameNature")
        {
            enemyCharacter[1] = Managers.Game.Spawn(Define.WorldObject.Monster, "Enemy_Rhino");
            NavMeshAgent nma1 = enemyCharacter[1].GetComponent<NavMeshAgent>();

            
        }
        else if (SceneManager.GetActiveScene().name == "5.InGameDesert")
        {
            enemyCharacter[2] = Managers.Game.Spawn(Define.WorldObject.Monster, "Vanguard");
            NavMeshAgent nma0 = enemyCharacter[2].GetComponent<NavMeshAgent>();

        }


        Vector3 randPos;

        //while(true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;
            //break;
            NavMeshPath path = new NavMeshPath();
            //yield return new WaitForEndOfFrame();
            //if (nma1.CalculatePath(randPos, path))
            //{
            //    break;
            //}
        }

        for(int i = 0; i < enemyCharacter.Length ; i++)
        {
            enemyCharacter[i].transform.position = randPos;
        }

        //obj1.transform.position = randPos;
        //obj2.transform.position = randPos;
        _reserveCount--;
    }
}
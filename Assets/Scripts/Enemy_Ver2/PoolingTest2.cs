using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PoolingTest2 : MonoBehaviour
{
    [SerializeField] int _enemyCount = 0;
    int _reserveCount = 0; // 코루틴을 생성할 때 현재 예약된 코루틴이 몇개인지 판단
    [SerializeField] int _keepEnemyCount = 0;
    [SerializeField] Vector3 _spawnPos = new Vector3(-30f, 0f, -80f);

    float _spawnRadius = 20.0f;
    float _spawnTime = 5.0f;

    public GameObject _enemy;

    public void AddMonsterCount(int value)
    {
        _enemyCount += value;
    }
    public void SetKeepMonsterCount(int count)
    {
        _keepEnemyCount = count;
    }

    // Start is called before the first frame update
    void Start()
    {
        Managers.Game.OnSpawnEvent_enemy2 -= AddMonsterCount;
        Managers.Game.OnSpawnEvent_enemy2 += AddMonsterCount;

    }

    // Update is called once per frame
    void Update()
    {
        while (_reserveCount + _enemyCount < _keepEnemyCount)
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
        if (SceneManager.GetActiveScene().name == "InGameNature")
        {
            _enemy = Managers.Game.Spawn(Define.WorldObject.Enemy2, "Enemy_Metalon");
            NavMeshAgent nma0 = _enemy.GetComponent<NavMeshAgent>();

        }
        else if (SceneManager.GetActiveScene().name == "InGameDesert")
        {
            _enemy = Managers.Game.Spawn(Define.WorldObject.Enemy2, "Enemy_DarkBlue");
            NavMeshAgent nma0 = _enemy.GetComponent<NavMeshAgent>();
        }
        else if (SceneManager.GetActiveScene().name == "InGameCemetery")
        {
            _enemy = Managers.Game.Spawn(Define.WorldObject.Enemy2, "Enemy_Skeleton");
            NavMeshAgent nma0 = _enemy.GetComponent<NavMeshAgent>();
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
        //for(int i = 0; i < enemyCharacter.Length ; i++)
        //{
        //    enemyCharacter[i].transform.position = randPos;
        //}
        _enemy.transform.position = randPos;

        _reserveCount--;
    }
}

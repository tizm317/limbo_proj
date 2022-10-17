using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField] int _monsterCount = 0;
    int _reserveCount = 0;
    [SerializeField] int _keepMonsterCount = 0;
    [SerializeField] Vector3 _spawnPos;

    float _spawnRadius = 15.0f;

    float _spawnTime = 5.0f;

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
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    // Update is called once per frame
    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));

        GameObject obj1 = Managers.Game.Spawn(Define.WorldObject.Monster, "Enemy_Wizard");
        NavMeshAgent nma1 = obj1.GetComponent<NavMeshAgent>();
        GameObject obj2 = Managers.Game.Spawn(Define.WorldObject.Monster, "Enemy_Monster");
        NavMeshAgent nma2 = obj2.GetComponent<NavMeshAgent>();


        Vector3 randPos;

        while(true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            NavMeshPath path = new NavMeshPath();
            //yield return new WaitForEndOfFrame();
            if (nma1.CalculatePath(randPos, path))
            {
                break;
            }
            if (nma2.CalculatePath(randPos, path))
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        obj1.transform.position = randPos;
        obj2.transform.position = randPos;

        _reserveCount--;
    }
}

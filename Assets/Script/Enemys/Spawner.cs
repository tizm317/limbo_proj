using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float timeToSpawn = 2f; 
    private float timeSinceSpawn;
    private ObjectPool objectPool;
    private int EnemyCount;
    [SerializeField] private int EnemyTotalCount;


    private Transform tr;  //enemy 위치
    private Transform playerTr;


    void Init()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();

        tr = GetComponent<Transform>();  //enemy 위치
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();    
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);

        if (dist <= 10.0f)
        {
            if (EnemyCount < EnemyTotalCount)
            {
                timeSinceSpawn += Time.deltaTime;
                if (timeSinceSpawn >= timeToSpawn)
                {
                    GameObject newEnemy = objectPool.GetEnemy();
                    newEnemy.transform.position = this.transform.position;
                    timeSinceSpawn = 0f;
                    EnemyCount++;
                }
            }
        }
    }
}

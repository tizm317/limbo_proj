using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject EnemyModel;
    [SerializeField]
    private Queue<GameObject> Pool = new Queue<GameObject>();
    [SerializeField]
    private int poolStartSize = 3;

    private void Start()
    {
        for (int i = 0; i < poolStartSize; i++)
        {
            GameObject Enemy = Instantiate(EnemyModel);
            Pool.Enqueue(Enemy);
            Enemy.SetActive(false);
        }
    }    
    public GameObject GetEnemy()
    {
        if (Pool.Count > 0)
        {
            GameObject Enemy = Pool.Dequeue();
            Enemy.SetActive(true);
            return Enemy;
        }
        else
        {
            GameObject Enemy = Instantiate(EnemyModel);
            return Enemy;
        }
    }
    
    public void ReturnEnemy(GameObject Enemy)
    {
        Pool.Enqueue(Enemy);
        Enemy.SetActive(false);
    }
}

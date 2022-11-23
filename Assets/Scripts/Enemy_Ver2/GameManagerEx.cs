using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    GameObject _player;
    HashSet<GameObject> _monsters = new HashSet<GameObject>();
    HashSet<GameObject> _enemy = new HashSet<GameObject>();
    HashSet<GameObject> _enemy2 = new HashSet<GameObject>();
    public Action<int> OnSpawnEvent;
    public Action<int> OnSpawnEvent_enemy;
    public Action<int> OnSpawnEvent_enemy2;
    public GameObject GetPlayer() { return _player; }

    //몬스터 생성
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);
        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if(OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);                    
                break;
            case Define.WorldObject.Enemy:
                _enemy.Add(go);
                if (OnSpawnEvent_enemy != null)
                    OnSpawnEvent_enemy.Invoke(1);
                break;
            case Define.WorldObject.Enemy2:
                _enemy2.Add(go);
                if (OnSpawnEvent_enemy2 != null)
                    OnSpawnEvent_enemy2.Invoke(1);
                break;
        }
        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        Enemy enemy = go.GetComponent<Enemy>();

        if(enemy == null)
        {
            return Define.WorldObject.Unknown;
        }
        return enemy.WorldObjectType;
    }

    //몬스터 삭제
    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go))
                    {
                        if (OnSpawnEvent != null)
                            OnSpawnEvent.Invoke(-1);
                        _monsters.Remove(go);
                    }
                    break;
                }
            case Define.WorldObject.Enemy:
                {
                    if (_enemy.Contains(go))
                    {
                        if (OnSpawnEvent_enemy != null)
                            OnSpawnEvent_enemy.Invoke(-1);
                        _enemy.Remove(go);
                    }
                    break;
                }
            case Define.WorldObject.Enemy2:
                {
                    if (_enemy2.Contains(go))
                    {
                        if (OnSpawnEvent_enemy2 != null)
                            OnSpawnEvent_enemy2.Invoke(-1);
                        _enemy2.Remove(go);
                    }
                    break;
                }
        }
        Managers.Resource.Destroy(go);
    }
}

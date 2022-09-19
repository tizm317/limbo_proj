using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    GameObject _player;
    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;
    public GameObject GetPlayer() { return _player; }
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if(OnSpawnEvent != null)
                {
                    OnSpawnEvent.Invoke(1);
                }
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

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch(type)
        {
            case Define.WorldObject.Monster:
                {
                    if(_monsters.Contains(go))
                    {
                        _monsters.Remove(go);
                        if(OnSpawnEvent!=null)
                        {
                            OnSpawnEvent.Invoke(-1);
                        }
                    }
                    break;
                }
        }
        Managers.Resource.Destroy(go);
    }
}

using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public Player MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            GameObject go = Managers.Resource.Instantiate("Character/MyWarrior");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            MyPlayer = go.GetComponent<MyWarrior>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.PosInfo = info.PosInfo;
        }
        else
        {
            GameObject go = Managers.Resource.Instantiate("Character/Warrior");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            Warrior p = go.GetComponent<Warrior>();
            p.Id = info.PlayerId;
            p.PosInfo = info.PosInfo;
        }
    }

    public void Add(int id, GameObject go)
    {
        _objects.Add(id, go);
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null) return;

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null) return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }


    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Vector3Int Pos)
    {
        foreach (GameObject go in _objects.Values)
        {
            Player player = go.GetComponent<Player>();
            if (player == null) continue;

            //if (player.Pos == Pos)
            //    return go;

        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject go in _objects.Values)
        {
            if (condition.Invoke(go))
                return go;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject go in _objects.Values)
            Managers.Resource.Destroy(go);
        
        _objects.Clear();
    }
}

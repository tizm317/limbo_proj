using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    //public MyPlayer MyPlayer { get; set; }
    //Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    //public void Add(PlayerInfo info, bool myPlayer = false)
    //{
    //    if(myPlayer)
    //    {
    //        GameObject go = Managers.Resource.Instantiate("Prefabs/Character/MyWarrior");
    //        go.name = info.Name;
    //        _objects.Add(info.PlayerId, go);

    //        MyPlayer = go.GetComponent<MyPlayer>();
    //        MyPlayer.Id = info.PlayerId;
    //        MyPlayer.Pos = new Vector3Int(info.PosX, 1, info.PosY);
    //    }
    //    else
    //    {
    //        GameObject go = Managers.Resource.Instantiate("Prefabs/Character/Warrior");
    //        go.name = info.Name;
    //        _objects.Add(info.PlayerId, go);

    //        Player p = go.GetComponent<Player>();
    //        p.Id = info.PlayerId;
    //        p.Pos = new Vector3Int(info.PosX, 1, info.PosY);
    //    }
    //}

    //public void Add(int id, GameObject go)
    //{
    //    _objects.Add(id, go);
    //}

    //public void Remove(int id)
    //{
    //    _objects.Remove(id);
    //}

    //public void RemoveMyPlayer()
    //{
    //    if (MyPlayer == null) return;

    //    Remove(MyPlayer.Id);
    //    MyPlayer = null;
    //}


    //public GameObject Find(Vector3Int Pos)
    //{
    //    foreach(GameObject go in _objects.Values)
    //    {
    //        Player player = go.GetComponent<Player>();
    //        if (player == null) continue;

    //        //if (player.Pos == Pos)
    //        //    return go;

    //    }

    //    return null;
    //}

    //public GameObject Find(Func<GameObject, bool> condition)
    //{
    //    foreach (GameObject go in _objects.Values)
    //    {
    //        if (condition.Invoke(go))
    //            return go;
    //    }

    //    return null;
    //}

    //public void Clear()
    //{
    //    _objects.Clear();
    //}
}

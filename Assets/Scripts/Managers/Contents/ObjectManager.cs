using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public Player MyPlayer { get; set; } // '나' 는 따로 관리하면 편함
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>(); // 서버쪽에서 부여받은 ID
    // 모든 오브젝트 하나로 관리할 수도 있고/ (_players/_monsters/_envs) 등으로 나눠서 관리할 수도 있음

    // Add, Remove, Find, Clear
    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        string job = "";
        switch (info.Job)
        {
            case 1:
                job = "Archer";
                break;
            case 2:
                job = "Sorcerer";
                break;
            default:
                job = "Warrior";
                break;
        }

        if (myPlayer)
        {
            GameObject go = Managers.Resource.Instantiate($"Character/My{job}");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            MyPlayer = go.GetComponent<Player>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.PosInfo = info.PosInfo;
            MyPlayer.DestInfo = info.DestInfo;
            MyPlayer.my_job = (Define.Job)info.Job;
            
            MyPlayer.transform.position = MyPlayer.Pos;
        }
        else // Not myPlayer
        {
            GameObject go = Managers.Resource.Instantiate($"Character/{job}");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            Player p = go.GetComponent<Player>();
            p.Id = info.PlayerId;
            p.PosInfo = info.PosInfo;
            p.DestInfo = info.DestInfo;
            p.my_job = (Define.Job)info.Job;

            p.transform.position = p.Pos;
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

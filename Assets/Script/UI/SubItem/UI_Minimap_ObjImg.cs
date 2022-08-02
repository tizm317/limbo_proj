using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Minimap_ObjImg : UI_Base
{
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        //Debug.Log("Init");
    }

    public void setMarkerPos(int idx)
    {
        // 위치 설정
        Vector3 tempVec;
        tempVec.x = Managers.Data.MapDict[idx].x;
        tempVec.y = Managers.Data.MapDict[idx].z;
        tempVec.z = 0;
        //Debug.Log(tempVec);
        gameObject.transform.localPosition = tempVec;
    }
}

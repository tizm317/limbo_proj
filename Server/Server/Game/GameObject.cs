﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        //공용으로 사용할 게임오브젝트 생성
        //proto 파일로 관리하면 클라이언트, packet에서 동일하게 사용할 수 있기에 편리
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public GameObject()
        {
            Info.PosInfo = PosInfo;
        }
    }
}
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo()};
        public GameRoom Room { get; set; }          // 어떤 Room 에 있는지
        public ClientSession Session { get; set; }  // 플레이어가 패킷 보낼 때 사용
    }
}

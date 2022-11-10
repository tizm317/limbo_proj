using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoom
    {
        // Room 하나 관리하는 객체

        object _lock = new object();                // 멀티 쓰레드 환경
        public int RoomId { get; set; }             // 씬(맵) 식별자
        List<Player> _players = new List<Player>(); // lock 걸어줘야 함

        // Game Room 입장/퇴장
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null) return;

            lock(_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 내 정보를 나한테
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 타인 정보를 나한테
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players)
                    {
                        if (newPlayer != p) // 나 빼고
                            spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }
                
                // 타인한테 정보 전송
                {
                    // 내 정보를 타인한테
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (Player p in _players)
                    {
                        if (newPlayer != p) // 나 빼고 Broadcasting
                            p.Session.Send(spawnPacket);
                    }
                }
            }
        }
        public void LeaveGame(int playerId)
        {
            lock(_lock)
            {
                // 원하는 플레이어 찾기
                Player player = _players.Find(p => p.Info.PlayerId == playerId);
                if (player == null) return;

                // 리스트에서 제거
                _players.Remove(player);
                player.Room = null;

                // 본인한테 (Leave)정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
                // 타인한테 (Despawn)정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(player.Info.PlayerId);
                    foreach (Player p in _players)
                    {
                        if (player != p) // 나 빼고 Broadcasting
                            p.Session.Send(despawnPacket);
                    }
                }
            }
        }

        // Broadcasting
        public void Broadcast(IMessage packet)
        {
            // ex) 채팅
            lock (_lock)
            {
                foreach(Player p in _players)
                {
                    p.Session.Send(packet);
                }
            }
        }
        public void BroadcastWithOutMyself(IMessage packet)
        {
            // ex) 이동동기화 : 내껀 클라에서 이동
            S_Move mp = packet as S_Move;
            
            lock (_lock)
            {
                foreach (Player p in _players)
                {
                    if (mp != null && mp.PlayerId == p.Info.PlayerId) 
                    {
                        // 이동동기화일 때는 나한테 안보낸다
                    }
                    else
                        p.Session.Send(packet);
                }
            }
        }
    }
}

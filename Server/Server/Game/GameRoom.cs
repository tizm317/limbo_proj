using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        List<Player> _players = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null) return;

            lock(_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 내 정보
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);
                    // 타인 정보
                    S_SPAWN spawnPacket = new S_SPAWN();
                    foreach (Player p in _players)
                    {
                        if (newPlayer != p) // 나 빼고
                            spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }
                
                // 타인한테 정보 전송
                {
                    S_SPAWN spawnPacket = new S_SPAWN();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (Player p in _players)
                    {
                        if (newPlayer != p) // 나 빼고
                            p.Session.Send(spawnPacket);
                    }
                }
            }
        }
        public void LeaveGame(int playerId)
        {
            lock(_lock)
            {
                Player player = _players.Find(p => p.Info.PlayerId == playerId);
                if (player == null) return;

                _players.Remove(player);
                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LEAVE_GAME leavePacket = new S_LEAVE_GAME();
                    player.Session.Send(leavePacket);
                }
                // 타인한테 정보 전송
                {
                    S_DESPAWN despawnPacket = new S_DESPAWN();
                    despawnPacket.PlayerIds.Add(player.Info.PlayerId);
                    foreach (Player p in _players)
                    {
                        if (player != p) // 나 빼고
                            p.Session.Send(despawnPacket);
                    }
                }
            }
        }
    }
}

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
                        {
                            spawnPacket.Players.Add(p.Info);
                        }
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

        // 이동 처리
        internal void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null) return;

            // 정보 수정은 한번에 한명씩 (경합상태 해결)
            lock(_lock)
            {
                // TODO : 검증

                // 다른 플레이어한테도 알려준다
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.PlayerId;
                resMovePacket.DestInfo = movePacket.DestInfo;   // destination의 좌표를 보내주는 것
                resMovePacket.PosInfo = movePacket.PosInfo;   // 플레이어 위치

                // 나 빼고 같은 Room 에 속한 사람들에게 Broadcasting
                // 나는 클라쪽에서 이동했기 때문.
                BroadcastWithOutMyself(resMovePacket);


                // 일단 서버에서 좌표 이동 (각자 클라에서 이동하도록 수정함)
                //PlayerInfo info = clientSession.MyPlayer.Info;
                //info.PosInfo = movePacket.PosInfo;
            }
        }

        internal void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null) return;

            lock (_lock)
            {
                PlayerInfo info = player.Info;

                // 스킬 쓸 수 있는 조건인지 체크(state?, cooltime? 등)
                // Idle 일 때만?
                if (info.PosInfo.State != State.Idle)
                    return;
                // TODO : 스킬 사용 가능 여부 체크

                // 통과
                info.DestInfo.State = State.Skill;

                S_Skill skill = new S_Skill{Info = new SkillInfo()};
                skill.PlayerId = info.PlayerId;
                skill.Info.SkillId = 1; // 나중에 데이터시트로 따로 관리(json, xml)
                Broadcast(skill);

                // 데미지 판정
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

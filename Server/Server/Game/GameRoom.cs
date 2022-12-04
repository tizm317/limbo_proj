using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        // Room 하나 관리하는 객체

        object _lock = new object();                // 멀티 쓰레드 환경
        public int RoomId { get; set; }             // 씬(맵) 식별자
        //List<Player> _players = new List<Player>(); // lock 걸어줘야 함

        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();


        public void Init()
        {
            // TODO
            // 맵 관련..?

            //TestTimer();

            //temp monster
            //Monster monster = ObjectManager.Instance.Add<Monster>();
            //이 사이에 (강의에서는) 몬스터의 위치 하드 코딩했음
            //EnterGame(monster);
        }
        public void TestTimer()
        {
            Console.WriteLine("TestTimer");
            PushAfter(100, TestTimer);
        }


        // 누군가 주기적으로 호출해줘야 함
        public void Update()
        {
            // TODO : Monster Update

            // Projectile Update
            lock (_lock)
            {
                foreach (Projectile projectile in _projectiles.Values)
                {
                    projectile.Update();
                }
            }

            // 일감처리
            Flush();
        }

        // Game Room 입장/퇴장
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null) 
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.id);

            // 본인한테 정보 전송
            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player player = gameObject as Player;

                    _players.Add(gameObject.id, player);
                    player.Room = this;

                    // 내 정보를 나한테
                    {
                        S_EnterGame enterPacket = new S_EnterGame();
                        enterPacket.Player = player.Info;
                        player.Session.Send(enterPacket);

                        // 타인 정보를 나한테
                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (Player p in _players.Values)
                        {
                            if (player != p) // 나 빼고
                            {
                                spawnPacket.Players.Add(p.Info);
                            }
                        }
                        player.Session.Send(spawnPacket);
                    }

                }
                else if(type == GameObjectType.Monster)
                {
                    Monster monster = gameObject as Monster;
                    _monsters.Add(gameObject.id, monster);
                    monster.Room = this;
                }
                else if(type == GameObjectType.Projectile)
                {
                    Projectile projectile = gameObject as Projectile;
                    _projectiles.Add(gameObject.id, projectile);
                    projectile.Room = this;
                }
                
            }

            // 타인한테 정보 전송
            {
                // 내 정보를 타인한테
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Players.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.id != gameObject.id) // 나 빼고 Broadcasting
                        p.Session.Send(spawnPacket);
                }
            }
        }
        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            lock(_lock)
            {
                if(type == GameObjectType.Player)
                {
                    //Player player = _players.Find(p => p.Info.ObjectId == playerId);
                    //if (player == null) return;

                    // 리스트에서 제거
                    //player.OnLeaveGame();
                    //_players.Remove(objectId);

                    Player player = null;
                    if (_players.Remove(objectId, out player) == false)
                        return;

                    player.Room = null;

                    // 본인한테 (Leave)정보 전송
                    {
                        S_LeaveGame leavePacket = new S_LeaveGame();
                        player.Session.Send(leavePacket);
                    }
                }
                else if(type == GameObjectType.Monster)
                {
                    Monster monster = null;
                    if (_monsters.Remove(objectId, out monster) == false)
                        return;
                    monster.Room = null;

                }
                else if(type == GameObjectType.Projectile)
                {
                    Projectile projectile = null;
                    if (_projectiles.Remove(objectId, out projectile) == false)
                        return;
                    projectile.Room = null;

                }
            }
            // 원하는 플레이어 찾기
            
            // 타인한테 (Despawn)정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.PlayerIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.id != objectId) // 나 빼고 Broadcasting
                        p.Session.Send(despawnPacket);
                }
            }
        }

        // 이동 처리
        internal void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null) return;

            // 정보 수정은 한번에 한명씩 (경합상태 해결)

            // 검증
            //PositionInfo movePosInfo = movePacket.PosInfo;
            //ObjectInfo info = player.Info;

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

        internal void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null) return;

            PlayerInfo info = player.Info;

            // 스킬 쓸 수 있는 조건인지 체크(state?, cooltime? 등)
            // Idle 일 때만?
            if (info.PosInfo.State != State.Idle)
                return;
            // TODO : 스킬 사용 가능 여부 체크

            // 통과(스킬을 사용하는 애니메이션 맞춰주기 위함)
            info.DestInfo.State = State.Skill;

            S_Skill skill = new S_Skill { Info = new SkillInfo() };
            skill.PlayerId = info.PlayerId;
            skill.Info.SkillId = skillPacket.Info.SkillId; // 1 -> skillPacket.Info.SkillId로 변경함 / 나중에 데이터시트로 따로 관리(json, xml)
            Broadcast(skill);

            // 스킬이 많은 경우 class로 따로 빼서 하는것이 효과적
            if (skillPacket.Info.SkillId == 1)
            {
                //TODO : 데미지 판정
            }
            else if (skillPacket.Info.SkillId == 2)
            {
                //TODO : Arrow
                
                Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                if (arrow == null)
                    return;

                arrow.Owner = player;
                arrow.PosInfo.State = State.Move;
                arrow.PosInfo.PosX = player.PosInfo.PosX;
                arrow.PosInfo.PosY = player.PosInfo.PosY;
                arrow.PosInfo.PosZ = player.PosInfo.PosZ;
                
                //타인에게 spawn 되었음을 보여주기 위함
                EnterGame(arrow);

            }
        }

        // Broadcasting
        public void Broadcast(IMessage packet)
        {
            // ex) 채팅
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
        public void BroadcastWithOutMyself(IMessage packet)
        {
            // ex) 이동동기화 : 내껀 클라에서 이동
            S_Move mp = packet as S_Move;
            foreach (Player p in _players.Values)
            {
                if (mp != null && mp.PlayerId == p.Info.PlayerId)
                {
                    // 이동동기화일 때는 나한테 안보낸다
                }
                else
                    p.Session.Send(packet);
            }
        }

        // Item
        internal void HandleInventory(Player player, C_ItemList itemList)
        {
            if (player == null) return;

            player.UpdateInventory(itemList);
        }
    }
}

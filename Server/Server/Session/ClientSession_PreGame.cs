using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using Server.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
		// PreGame (인게임 이전)

		public int AccountDbId { get; private set; }

		// 현재 로비에서 들고 있는 플레이어 목록
		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

        public void HandleLogin(C_Login loginPacket)
        {
			// TODO : 이런 저런 보안 체크
			// 1. State 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;

			// 혹시 HandleLogin 여러번 실행될 수 있으니 clear 먼저 해줌
			LobbyPlayers.Clear();

			// TODO : 문제가 있긴 있다
			// - 동시에 다른 사람들이 같은 UniqueId를 보낸다면?
			// - 악의적으로 여러번 보낸다면?
			// - 생뚱맞은 타이밍에 그냥 이 패킷을 보낸다면?

			// 해당하는 Account 있는지 체크
			using (AppDbContext db = new AppDbContext())
			{
				// 계정db에 있는지 찾아보고,
				AccountDb findAccount = db.Accounts
					.Include(a => a.Players) // Account와 연동된 플레이어들 가져오기
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

				if (findAccount != null) // 이전에 만든 플레이어가 있다
				{
					// AccountDbId 메모리에 기억
					AccountDbId = findAccount.AccountDbId;

					// 있으면 OK 패킷 보냄
					S_Login loginOk = new S_Login() { LoginOk = 1 };
					
					// 플레이어 정보 같이 보냄
					foreach (PlayerDb playerDb in findAccount.Players)
                    {
						LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
						{
							PlayerDbId = playerDb.PlayerDbId,
							Name = playerDb.PlayerName,
							Job = playerDb.PlayerJob,
							PlayerStatInfo = new PlayerStatInfo()
							{
								// Db에서 가져와서 넣어줌
								//StatInfo = new StatInfo()
                                //{
								Level = playerDb.Level,
								Hp = playerDb.Hp,
								MaxHp = playerDb.MaxHp,
								Attack = playerDb.Attack,
								Defence = playerDb.Defence,
								MoveSpeed = playerDb.MoveSpeed,
								TurnSpeed = playerDb.TurnSpeed,
								AttackSpeed = playerDb.AttackSpeed,
								//},
								Exp = playerDb.Exp,
								Gold = playerDb.Gold,
								NextLevelUp = playerDb.Next_level_up,
								CurrentExp = playerDb.Current_exp,
								Regeneration = playerDb.Regeneration,
								Mana = playerDb.Mana,
								MaxMana = playerDb.Max_mana,
								ManaRegeneration = playerDb.Mana_regeneration,
								SkillPoint = playerDb.Skill_point
                            }
						};

						// 메모리에 들고 있는다 (장점 : DB 접근 한번만 해서 성능 up)
						LobbyPlayers.Add(lobbyPlayer);

						// 패킷에 넣어준다
						loginOk.Players.Add(lobbyPlayer);
                    }
					Send(loginOk);

					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
				else // 이전에 만든 플레이어가 없다
				{
					// 이전에 만든 플레이어가 없다는 정보만 보내주면 됨!!
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					bool success = db.SaveChangesEx(); // TODO: Exception
					if (success == false) return;

					// AccountDbId 메모리에 기억
					AccountDbId = newAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);

					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
			}
			//Console.WriteLine($"UniqueId({loginPacket.UniqueId})");
		}
		public void HandleEnterGame(C_EnterGame enterGamePacket)
		{
			// 로비에서만 가능
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			// 접속하고 싶은 캐릭터 이름이랑 겹치는 캐릭터 찾기
			LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
			if (playerInfo == null) return;

			MyPlayer = PlayerManager.Instance.Add();
			{   // 정보 셋팅
				MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
				MyPlayer.Info.Name = playerInfo.Name; 

				// 플레이어 위치 (TODO : 나중에 DB에서 가져옴 / 종료될 때 저장함)
				MyPlayer.Info.PosInfo.State = State.Idle;
				MyPlayer.Info.PosInfo.PosX = -25f;
				MyPlayer.Info.PosInfo.PosY = 0; // 높이
				MyPlayer.Info.PosInfo.PosZ = -30f;

				// 목적지 위치
				MyPlayer.Info.DestInfo.State = State.Idle;
				MyPlayer.Info.DestInfo.PosX = -25f; //1.2
				MyPlayer.Info.DestInfo.PosY = 0;
				MyPlayer.Info.DestInfo.PosZ = -30f;

				// PlayerStatInfo
				MyPlayer.StatInfo.MergeFrom(playerInfo.PlayerStatInfo);

				// Job
				MyPlayer.Info.Job = playerInfo.Job;

				// session
				MyPlayer.Session = this;

				S_ItemList itemListPacket = new S_ItemList();

				// 아이템 목록 갖고 오기
				using (AppDbContext db = new AppDbContext())
                {
					List<ItemDb> items = db.Items
						.Where(i=> i.OwnerDbId == playerInfo.PlayerDbId)
						.ToList();

					foreach(ItemDb itemDb in items)
                    {
						// itemDb 정보로 item 만듦
						Item item = Item.MakeItem(itemDb);

                        // 인벤토리
						if(item != null)
                        {
							// 인벤토리 추가
							MyPlayer.Inven.Add(item);
						
							// 패킷에 추가
							ItemInfo info = new ItemInfo();
							info.MergeFrom(item.Info); // 해당 아이템 정보 넣어줌
							itemListPacket.Items.Add(info);
						}
					}
				}
				Send(itemListPacket);
			}

			// State 변경
			ServerState = PlayerServerState.ServerStateGame;

			// Room 1 찾아서 입장시킴
			GameRoom room = RoomManager.Instance.Find(1);
			room.Push(room.EnterGame, MyPlayer);
		}
		public void HandleCreatePlayer(C_CreatePlayer createPacket)
        {
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			// 플레이어 생성
			using (AppDbContext db = new AppDbContext())
            {
				PlayerDb findPlayer = db.Players
					.Where(p => p.PlayerName == createPacket.Name).FirstOrDefault(); // 같은 이름 있는지 찾기

				if(findPlayer != null)
                {
					// 이름이 겹친다 -> 빈 값으로 보냄
					Send(new S_CreatePlayer());
                }
				else // 이름이 안 겹침 -> 새 플레이어 만들기
                {
					// 1레벨 스탯 정보 추출(데이터 시트에서 읽어옴)
					//StatInfo stat = null;
					PlayerStatInfo playerStat = null;
					DataManager.PlayerStatDict.TryGetValue(1, out playerStat); // 1레벨 정보 추출

					// DB에 플레이어 만들기
					PlayerDb newPlayerDb = new PlayerDb()
					{
						// 정보 추가
						PlayerName = createPacket.Name,
						PlayerJob = createPacket.Job,

						Level = playerStat.Level,
						Hp = playerStat.Hp,
						MaxHp = playerStat.MaxHp,
						Attack = playerStat.Attack,
						Defence = playerStat.Defence,
						MoveSpeed = playerStat.MoveSpeed,
						AttackSpeed = playerStat.AttackSpeed,
						TurnSpeed = playerStat.TurnSpeed,

						Exp = playerStat.Exp,
						Gold = playerStat.Gold,
						Next_level_up = playerStat.NextLevelUp,
						Current_exp = playerStat.CurrentExp,
						Regeneration = playerStat.Regeneration,
						Mana = playerStat.Mana,
						Max_mana = playerStat.MaxMana,
						Mana_regeneration = playerStat.ManaRegeneration,
						Skill_point = playerStat.SkillPoint,

						AccountDbId = AccountDbId
					};

					db.Players.Add(newPlayerDb);
					bool success = db.SaveChangesEx(); // TODO : ExceptionHandling (이름 겹치는 문제(player 이름이 찰나의 순간에 다른 사람이 선점))
					if (success == false) return;

					// 메모리에 추가
					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						PlayerDbId = newPlayerDb.PlayerDbId,
						Name = createPacket.Name,
						Job = createPacket.Job,
						PlayerStatInfo = new PlayerStatInfo()
						{
							// Db에서 가져와서 넣어줌
							//StatInfo = new StatInfo()
							//{
							Level = playerStat.Level,
							Hp = playerStat.Hp,
							MaxHp = playerStat.MaxHp,
							Attack = playerStat.Attack,
							Defence = playerStat.Defence,
							MoveSpeed = playerStat.MoveSpeed,
							AttackSpeed = playerStat.AttackSpeed,
							TurnSpeed = playerStat.TurnSpeed,
							//},
							Exp = playerStat.Exp,
							Gold = playerStat.Gold,
							NextLevelUp = playerStat.NextLevelUp,
							CurrentExp = playerStat.CurrentExp,
							Regeneration = playerStat.Regeneration,
							Mana = playerStat.Mana,
							MaxMana = playerStat.MaxMana,
							ManaRegeneration = playerStat.ManaRegeneration,
							SkillPoint = playerStat.SkillPoint
						}
					};

					// 메모리에 들고 있는다 (장점 : DB 접근 한번만 해서 성능 up)
					LobbyPlayers.Add(lobbyPlayer);

					// 클라에 전송 (정보 넣어서)
					S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
					newPlayer.Player.MergeFrom(lobbyPlayer);

					Send(newPlayer);
				}
            }
		}

	}
}

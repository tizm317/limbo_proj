using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
		public int AccountDbId { get; private set; }
		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

        public void HandleLogin(C_Login loginPacket)
        {
			//Console.WriteLine($"UniqueId({loginPacket.UniqueId})");
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;

			LobbyPlayers.Clear();

			// TODO : 문제가 있긴 있다
			// 해당하는 Account 있는지 체크
			using (AppDbContext db = new AppDbContext())
			{
				// 계정db에 있는지 찾아보고,
				AccountDb findAccount = db.Accounts
					.Include(a => a.Players) // Account 연동 플레이어들 가져오기
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

				if (findAccount != null)
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
							Name = playerDb.PlayerName,
							Job = playerDb.PlayerJob,
							StatInfo = new StatInfo()
							{
								Level = playerDb.Level,
								Hp = playerDb.Hp,
								MaxHp = playerDb.MaxHp,
								Attack = playerDb.Attack,
								Speed = playerDb.Speed,
								TotalExp = playerDb.TotalExp
                            }
						};

						// 메모리에 들고 있는다 (DB 접근 한번만)
						LobbyPlayers.Add(lobbyPlayer);

						// 패킷에 넣어준다
						loginOk.Players.Add(lobbyPlayer);
                    }
					Send(loginOk);

					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
				else
				{
					// 없으면 db에 저장
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					db.SaveChanges(); // TODO: Exception

					// AccountDbId 메모리에 기억
					AccountDbId = newAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);

					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
			}
		}

		public void HandleEnterGame(C_EnterGame enterGamePacket)
		{
			// 로비에서만 가능
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			// 접속하고 싶은 캐릭터 이름이랑 겹치는 캐릭터 찾기
			LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
			if (playerInfo == null) return;

			/////////////////////////////////////////////////////////////////////////
			//MyPlayer = PlayerManager.Instance.Add();
			//{   // 정보 셋팅
			//	MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}"; // 임시
			//	MyPlayer.Info.PosX = 0;
			//	MyPlayer.Info.PosY = 0;
			//	MyPlayer.Session = this;
			//}

			//RoomManager.Instance.Find(1).EnterGame(MyPlayer);
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
					// 이름이 겹친다
					Send(new S_CreatePlayer());
                }
				else
                {
					// 1레벨 스탯 정보 추출
					StatInfo stat = null;
					DataManager.StatDict.TryGetValue(1, out stat);

					// DB에 플레이어 만들기
					PlayerDb newPlayerDb = new PlayerDb()
					{
						PlayerName = createPacket.Name,
						Level = stat.Level,
						Hp = stat.Hp,
						Attack = stat.Attack,
						Speed = stat.Speed,
						TotalExp = 0,
						AccountDbId = AccountDbId
					};

					db.Players.Add(newPlayerDb);
					db.SaveChanges(); // TODO : ExceptionHandling (이름 겹치는 문제)

					// 메모리에 추가
					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						Name = createPacket.Name,
						StatInfo = new StatInfo()
						{
							Level = stat.Level,
							Hp = stat.Hp,
							MaxHp = stat.MaxHp,
							Attack = stat.Attack,
							Speed = stat.Speed,
							TotalExp = 0
						}
					};

					// 메모리에 들고 있는다 (DB 접근 한번만)
					LobbyPlayers.Add(lobbyPlayer);

					// 클라에 전송
					S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
					newPlayer.Player.MergeFrom(lobbyPlayer);

					Send(newPlayer);
				}
            }
		}

	}
}

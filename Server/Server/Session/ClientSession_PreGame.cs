using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
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
        public void HandleLogin(C_Login loginPacket)
        {
			//Console.WriteLine($"UniqueId({loginPacket.UniqueId})");
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;

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
					// 있으면 OK 패킷 보냄
					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);
				}
				else
				{
					// 없으면 db에 저장
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					db.SaveChanges(); // TODO: Exception

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);
				}
			}
		}
    }
}

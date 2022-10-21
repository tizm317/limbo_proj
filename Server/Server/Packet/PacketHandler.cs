using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_MOVE movePacket = packet as C_MOVE;
		ClientSession serverSession = session as ClientSession;
	}


	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login loginPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"UniqueId({loginPacket.UniqueId})");

		// TODO : 이런 저런 보안 체크

		// TODO : 문제가 있긴 있다
		// 해당하는 Account 있는지 체크
		using(AppDbContext db = new AppDbContext())
        {
			// 계정db에 있는지 찾아보고,
			AccountDb findAccount = db.Accounts
				.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

			if(findAccount != null)
            {
				// 있으면 OK 패킷 보냄
				S_Login loginOk = new S_Login() { LoginOk = 1 };
				clientSession.Send(loginOk);
			}
			else
            {
				// 없으면 db에 저장
				AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
				db.Accounts.Add(newAccount);
				db.SaveChanges();

				S_Login loginOk = new S_Login() { LoginOk = 1 };
				clientSession.Send(loginOk);
			}
        }
	}
}

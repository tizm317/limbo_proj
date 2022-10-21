using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();

		static void FlushRoom()
		{
			JobTimer.Instance.Push(FlushRoom, 250);
		}

		static void Main(string[] args)
		{
			ConfigManager.LoadConfig();
			//

			// DB Test // DB를 컨텐츠 코드에서 바로 접근하는 것도 문제(오래 걸리면 .. 다른 부분도 오래걸림)
			using (AppDbContext db = new AppDbContext())
            {
				// Entity framework Core에서는 AppDbContext를 단기적으로 만들고 날려버리는 게 정석
				// 필요할 때마다 함(비용도 별로 안 듦) (cf AppDbContext를 풀링 할 수도 있음)
				// using 은 dispose 자동
				db.Accounts.Add(new AccountDb() { AccountName = "TestAccount" });
				db.SaveChanges(); // exception Handling 을 해야함
            }

			RoomManager.Instance.Add();

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			//FlushRoom();
			JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				JobTimer.Instance.Flush();
			}
		}
	}
}

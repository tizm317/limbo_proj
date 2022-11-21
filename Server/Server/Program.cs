using System;
using System.Collections.Generic;
using System.Linq;
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
using Server.Utils;
using ServerCore;
using SharedDB;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>(); // 주기적으로 도는 타이머들(혹시 멈추고 싶을 경우)

		static void TickRoom(GameRoom room, int tick = 100)
        {
			var timer = new System.Timers.Timer();
			timer.Interval = tick; // 몇 틱마다 실행
			timer.Elapsed += ((s, e) => { room.Update(); }); // 시간 지나면 무엇을 실행시킬지
			timer.AutoReset = true; // 매번마다 리셋
			timer.Enabled = true;

			_timers.Add(timer);
        }

		static void TickDb(int tick = 100)
		{
			var timer = new System.Timers.Timer();
			timer.Interval = tick; // 몇 틱마다 실행
			timer.Elapsed += ((s, e) => { DbTransaction.Instance.Flush(); });
			timer.AutoReset = true; // 매번마다 리셋
			timer.Enabled = true;

			_timers.Add(timer);
		}

		static void StartServerInfoTask()
        {
			var t = new System.Timers.Timer();
			t.AutoReset = true;
			t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
			{
				using (SharedDbContext shared = new SharedDbContext())
				{
					ServerDb serverDb = shared.Servers.Where(s => s.Name == Name).FirstOrDefault();
					if (serverDb != null)
					{
						serverDb.IpAddress = IpAddress;
						serverDb.Port = Port;
						serverDb.BusyScore = SessionManager.Instance.GetBusyScore();
						serverDb.Open = Open;
						shared.SaveChangesEx();
					}
					else
					{
						serverDb = new ServerDb()
						{
							Name = Program.Name,
							IpAddress = Program.IpAddress,
							Port = Program.Port,
							BusyScore = SessionManager.Instance.GetBusyScore(),
							Open = Program.Open,
						};
						shared.Servers.Add(serverDb);
						shared.SaveChangesEx();
					}
				}
			});
			t.Interval = 10 * 1000;
			t.Start();
        }

		public static string Name { get; } = "Hongik";
		public static int Port { get; } = 7777;
		public static string IpAddress { get; set; }
		public static int Open { get; } = 1;

		static void Main(string[] args)
		{
			// Config파일, 데이터 로드
			ConfigManager.LoadConfig();
			DataManager.LoadData();


			//// DB Test // DB를 컨텐츠 코드에서 바로 접근하는 것도 문제(오래 걸리면 .. 다른 부분도 오래걸림)
			//using (AppDbContext db = new AppDbContext())
			//         {
			//	// Entity framework Core에서는 AppDbContext를 단기적으로 만들고 날려버리는 게 정석
			//	// 필요할 때마다 함(비용도 별로 안 듦) (cf AppDbContext를 풀링 할 수도 있음)
			//	// using 은 dispose 자동
			//	db.Accounts.Add(new AccountDb() { AccountName = "TestAccount" });
			//	db.SaveChanges(); // exception Handling 을 해야함
			//         }

			// 게임룸 1개 생성 -> TODO 나중에는 데이터로 빼서, 시작지역 몇번에서 시작하고 정해줘야함!!
			// 일단은 1번룸만 사용한다고 가정
			GameRoom room =  RoomManager.Instance.Add();

			// 
            {
				TickRoom(room, 50); // 50틱마다 한번씩 실행되도록 예약
				TickDb(50); // 50틱 마다 Db Job Flush 실행하도록 예약
			}


			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[1]; // 1로 수정함
			IPEndPoint endPoint = new IPEndPoint(ipAddr, Port);

			IpAddress = ipAddr.ToString();

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			StartServerInfoTask();

			//FlushRoom();
			//JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				//JobTimer.Instance.Flush();
				Thread.Sleep(100); // 프로그램 꺼지지 않게만 유지
			}
		}
	}
}

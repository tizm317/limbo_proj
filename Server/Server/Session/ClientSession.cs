using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

		public Player MyPlayer { get; set; } // Session이 관리하는 Player(Room) 알면 편함 (ex : 내 Room 찾아서 나갈 때 접근)
		public int SessionId { get; set; }


        #region Network
        public void Send(IMessage packet)
        {
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
		
			Send(new ArraySegment<byte>(sendBuffer));
		}

		// 연결 이후 작업
		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// 연결됨을 알림. To Client
            {
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
            }

			// 캐릭터 만드는 부분 PreGame쪽으로 이동함
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		// 연결 끊긴 이후 작업
		public override void OnDisconnected(EndPoint endPoint)
		{
			// Room 1에서 Leave시킴
			GameRoom room = RoomManager.Instance.Find(1);
			room.Push(room.LeaveGame, MyPlayer.Info.PlayerId);

			// 세션 제거
			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}

		#endregion
	}
}

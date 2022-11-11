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

            // TODO : 로비에서 캐릭터 선택
            // PROTO Test
            MyPlayer = PlayerManager.Instance.Add();
            {   // 정보 셋팅
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}"; // 임시 (나중에는 DB에서)
				
				// 플레이어 위치 (TODO : 나중에 DB에서 가져옴 / 종료될 때 저장함)
				MyPlayer.Info.PosInfo.State = State.Idle;
				MyPlayer.Info.PosInfo.PosX = 1.2f;
				MyPlayer.Info.PosInfo.PosY = 1; // 높이
				MyPlayer.Info.PosInfo.PosZ = -62.6f;

				// 목적지 위치
				MyPlayer.Info.DestInfo.State = State.Idle;
				MyPlayer.Info.DestInfo.PosX = 1.2f;
				MyPlayer.Info.DestInfo.PosY = 1; // 높이
				MyPlayer.Info.DestInfo.PosZ = -62.6f;

				MyPlayer.Info.Job = 0;
				MyPlayer.Session = this;
				//MyPlayer.Info.Destinations.Clear();
            }

            //// TODO : 입장 요청 들어오면
			// Room 1 찾아서 입장시킴
            RoomManager.Instance.Find(1).EnterGame(MyPlayer);
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		// 연결 끊긴 이후 작업
		public override void OnDisconnected(EndPoint endPoint)
		{
			// Room 1에서 Leave시킴
			RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.PlayerId);

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

﻿using System;
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

		public Player MyPlayer { get; set; }
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

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// 연결됨을 알림.
            {
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
            }

            // TODO : 로비에서 캐릭터 선택
            // PROTO Test
            MyPlayer = PlayerManager.Instance.Add();
            {   // 정보 셋팅
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}"; // 임시 (나중에는 DB에서)
				MyPlayer.Info.PosInfo.State = State.Idle;
				MyPlayer.Info.PosInfo.PosX = 1.2f;
				MyPlayer.Info.PosInfo.PosY = 1; // 높이
				MyPlayer.Info.PosInfo.PosZ = -62.6f;
                MyPlayer.Session = this;
				MyPlayer.Info.Destinations.Clear();
            }

            //// TODO : 입장 요청 들어오면
            RoomManager.Instance.Find(1).EnterGame(MyPlayer);


        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.PlayerId);

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

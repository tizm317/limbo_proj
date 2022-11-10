using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
	public int AccountId { get; set; } 
	public int Token { get; set; } 

	ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void ConnectToGame(ServerInfo info)
	{
        // DNS (Domain Name System)
        //string host = Dns.GetHostName();
		// Hoyoung's laptop host name = "DESKTOP-SD8FC1H"
		// Hoyoung's Destop host name = "DESKTOP-MOAPUEA"
		//string host = "DESKTOP-MOAPUEA";
		//IPHostEntry ipHost = Dns.GetHostEntry(host);
		//IPAddress ipAddr = ipHost.AddressList[1];

		IPAddress ipAddr = IPAddress.Parse(info.IpAddress);
		IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

		Connector connector = new Connector();
		
		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

	public void OnUpdate()
	{
		// 패킷 큐에 넣은 거 다 꺼내서 처리하는 과정
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}
	}

}

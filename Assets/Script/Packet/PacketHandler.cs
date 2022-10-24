﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_EnterGameHandler");
		Debug.Log(enterGamePacket.Player);
	}
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LEAVE_GAME leaveGamePacket = packet as S_LEAVE_GAME;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_LeaveGameHandler");
	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_SPAWN spawnPacket = packet as S_SPAWN;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_SpawnHandler");
		Debug.Log(spawnPacket.Players);
	}
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_DESPAWN despawnPacket = packet as S_DESPAWN;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_DespawnHandler");
	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_MOVE movePacket = packet as S_MOVE;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_MoveHandler");
	}

	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("S_ConnectedHandler");
		C_Login loginPacket = new C_Login();
		loginPacket.UniqueId = SystemInfo.deviceUniqueIdentifier; // unique key
		Managers.Network.Send(loginPacket);
	}

	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = (S_Login)packet;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");
	}

	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createPlayerPacket = (S_CreatePlayer)packet;
		ServerSession serverSession = (ServerSession)session;
	}
}

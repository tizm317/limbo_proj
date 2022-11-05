using DuloGames.UI;
using Google.Protobuf;
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
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_EnterGameHandler");
		//Debug.Log(enterGamePacket.Player);

		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_LeaveGameHandler");

		Managers.Object.RemoveMyPlayer();
	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_SpawnHandler");
		//Debug.Log(spawnPacket.Players);

		foreach(PlayerInfo player in spawnPacket.Players)
        {
			Managers.Object.Add(player, myPlayer: false);
        }
	}
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_DespawnHandler");

		foreach (int id in despawnPacket.PlayerIds)
		{
			Managers.Object.Remove(id);
		}
	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindById(movePacket.PlayerId);
		if (go == null) return;

		Player p = go.GetComponent<Player>();
		if (p == null) return;

		p.PosInfo = movePacket.PosInfo;
		p.Set_Destination(p.Dest);
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

	public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chatPacket = (S_Chat)packet;
		Debug.Log($"{chatPacket.PlayerId} : {chatPacket.ChatMessage}");

		Demo_Chat UI_Chat = GameObject.Find("Chat").GetComponentInChildren<Demo_Chat>();
		string text = $"PlayerId_{chatPacket.PlayerId} : {chatPacket.ChatMessage}";
		UI_Chat.ReceiveChatMessage(chatPacket);
		//UI_Chat.ReceiveChatMessage(1, text);
	}
}

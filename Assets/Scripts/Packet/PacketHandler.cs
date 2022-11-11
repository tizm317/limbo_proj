using DuloGames.UI;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	// 패킷 핸들러 : 서버로부터 받은 패킷 대응

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);

		//ServerSession serverSession = session as ServerSession;
		//Debug.Log("S_EnterGameHandler");
		//Debug.Log(enterGamePacket.Player);
	}
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		Managers.Object.RemoveMyPlayer();

		//ServerSession serverSession = session as ServerSession;
		//Debug.Log("S_LeaveGameHandler");
	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach(PlayerInfo player in spawnPacket.Players)
        {
			Managers.Object.Add(player, myPlayer: false);
        }

		//ServerSession serverSession = session as ServerSession;
		//Debug.Log("S_SpawnHandler");
		//Debug.Log(spawnPacket.Players);
	}
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.PlayerIds)
		{
			Managers.Object.Remove(id);
		}

		//ServerSession serverSession = session as ServerSession;
		//Debug.Log("S_DespawnHandler");
	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		//ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindById(movePacket.PlayerId);
		if (go == null) return;

		Player p = go.GetComponent<Player>();
		if (p == null) return;

		// 목적지 위치 넣어줌
		p.DestInfo = movePacket.DestInfo;
		p.Set_Destination(p.Dest);
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.PlayerId);
		if (go == null) return;

		Player p = go.GetComponent<Player>();
		if (p == null) return;

		p.UseSkill(skillPacket.Info.SkillId);
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

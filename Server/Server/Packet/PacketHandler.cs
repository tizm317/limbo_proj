using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketHandler
{
	// 멀티쓰레드 환경 (위험)

	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"C_Move Destiantion ({movePacket.DestInfo.PosX}, {movePacket.DestInfo.PosY}, {movePacket.DestInfo.PosZ})");

		// 멀티쓰레드 환경에서 위험하기 때문에 꺼내서 체크한다.
		Player player = clientSession.MyPlayer;
		if (player == null) return;
		GameRoom room = clientSession.MyPlayer.Room;
		if (room == null) return;

		// Room 안에서 처리하도록 함
		room.HandleMove(player, movePacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		// 멀티쓰레드 환경에서 위험하기 때문에 꺼내서 체크한다.
		Player player = clientSession.MyPlayer;
		if (player == null) return;
		GameRoom room = clientSession.MyPlayer.Room;
		if (room == null) return;

		room.HandleSkill(player, skillPacket);
	}

	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login loginPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;

		clientSession.HandleLogin(loginPacket);
	}

	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
		C_EnterGame enterGamePacket = (C_EnterGame)packet;
		ClientSession clientSession = (ClientSession)session;


		clientSession.HandleEnterGame(enterGamePacket);
	}

	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
		ClientSession clientSession = (ClientSession)session;

		clientSession.HandleCreatePlayer(createPlayerPacket);
	}

	public static void C_ChatHandler(PacketSession session, IMessage packet)
	{
		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;

		Console.WriteLine($"C_Chat ({chatPacket.PlayerId} : {chatPacket.ChatMessage})");

		if (clientSession.MyPlayer == null)
			return;
		if (clientSession.MyPlayer.Room == null)
			return;

		// TODO : 검증

		// 다른 플레이어한테도 알려준다
		S_Chat resChatPacket = new S_Chat();
		resChatPacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
		resChatPacket.ChatMessage = chatPacket.ChatMessage;

		clientSession.MyPlayer.Room.Broadcast(resChatPacket);
	}
}

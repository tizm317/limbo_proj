using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY}, {movePacket.PosInfo.PosZ})");

		if (clientSession.MyPlayer == null)
			return;
		if (clientSession.MyPlayer.Room == null)
			return;

		// TODO : 검증
		
		// 일단 서버에서 좌표 이동
		PlayerInfo info = clientSession.MyPlayer.Info;
		info.PosInfo = movePacket.PosInfo;

		// 다른 플레이어한테도 알려준다
		S_Move resMovePacket = new S_Move();
		resMovePacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
		resMovePacket.PosInfo = movePacket.PosInfo;
		
		clientSession.MyPlayer.Room.Broadcast(resMovePacket);
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

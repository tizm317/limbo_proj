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

	// 로그인 OK + 캐릭터 목록
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = (S_Login)packet;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");

        // TODO : 로비 UI 에서 캐릭터 보여주고, 선택할 수 있도록
        {
			LobbyScene lobby = GameObject.Find("@Scene").GetComponent<LobbyScene>();
			lobby.GetInfo(loginPacket);
		}


		// 일단은 플레이어 있는지 없는지만 구분하고 -> 
		//if(loginPacket.Players == null || loginPacket.Players.Count == 0)
  //      {
		//	// 없으면 새로 만들어 달라고 요청(응답은 S_CreatePlayerHandler로)
		//	C_CreatePlayer createPacket = new C_CreatePlayer();
		//	createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}"; //  format 0000 으로 맞춤
		//	Managers.Network.Send(createPacket);
  //      }
		//else // 플레이어 있는 경우
  //      {
		//	// 임시 : 무조건 첫번째 로그인
		//	LobbyPlayerInfo info = loginPacket.Players[0];
		//	C_EnterGame enterGamePacket = new C_EnterGame();
		//	enterGamePacket.Name = info.Name;
		//	Managers.Network.Send(enterGamePacket);
  //      }
	}
	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;

		if(createOkPacket.Player == null)
        {
			// 어떤 사유로 인해 만드는데 실패함 -> 다른 랜덤값으로 시도
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}"; //  format 0000 으로 맞춤
			Managers.Network.Send(createPacket);
		}
		else
        {
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			Managers.Network.Send(enterGamePacket);
		}
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

	public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
		S_ItemList itemList = (S_ItemList)packet;

		UI_InGame gameSceneUI =  Managers.UI.SceneUI as UI_InGame;
		UI_Inventory invenUI = gameSceneUI.uI_Inventory;

		//Managers.Inven.Clear();


		// 메모리에 아이템 정보 적용
		foreach(ItemInfo itemInfo in itemList.Items)
        {
            ItemData item = null;
            item = invenUI._inventory.itemDict[itemInfo.TemplateId];

            int idx;
            invenUI._inventory.Add(item, out idx);


            //Item2 item = Item2.MakeItem(itemInfo);
            //Managers.Inven.Add(item);

            //Debug.Log($"{item.TemplateId} : {item.Count}");
        }

		// UI에서 표시
		//invenUI.gameObject.SetActive(true);
		//invenUI.RefreshUI();
	}
}

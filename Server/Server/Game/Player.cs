using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.DB;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public GameRoom Room { get; set; }          // 어떤 Room 에 있는지
        public ClientSession Session { get; set; }  // 플레이어가 패킷 보낼 때 사용
        public Inventory Inven { get; private set; } = new Inventory();

        
        public PositionInfo DestInfo { get; set; } = new PositionInfo();
        public PlayerStatInfo StatInfo { get; set; } = new PlayerStatInfo();

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Info.DestInfo = DestInfo;
            Info.PlayerStatInfo = StatInfo;
        }

        // DB 연동?
        // 1) 피가 깎일 때마다 DB 접근할 필요가 있을까?
        // -> 방 나갈 때마다 DB 갱신
        // 2) 서버 다운되면 아직 저장되지 않은 정보 날아감
        // 3) 코드 흐름을 다 막아버린다 !!!
        // -> 비동기(Async) 방법 사용?
        // -> 다른 쓰레드로 DB 일감을 던저버리면 되지 않을까?
        //  -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음
        //  -- 아이템 생성 (DB 저장되지 않은 상태에서 이어서 작업하는건 문제가 된다)
        //      ->  결론 : 다른 애들한테 일감을 던지는 건 맞고 + 일감이 끝나면 통보 받고 이어서 작업해야함
        
        // 비유
        // 게임룸      : 서빙 담당 (클라 끼리 교류)
        // 디비 작업   : 결제 담당
        public void OnLeaveGame()
        {
            // Room 떠날 때 Db 갱신 작업
            DbTransaction.SavePlayerStatus_Step1(this, Room);
        }

        public void UpdateInventory(C_ItemList itemList)
        {
            DbTransaction.SavePlayerInventory_Step1(this, itemList, Room);
        }
    }
}

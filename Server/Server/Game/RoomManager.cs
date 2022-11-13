using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class RoomManager
    {
        // 모든 Room 관리하는 객체

        public static RoomManager Instance { get; } = new RoomManager();    // 싱글톤
        object _lock = new object();    // 멀티 쓰레드 환경
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>(); // dictionary로 모든 게임 Room Id로 관리 , lock걸어줘야 함
        int _roomId = 1;                // 1번부터 카운팅

        // 게임룸 생성(추가) , 제거, 찾기
        public GameRoom Add()
        {
            // 게임룸 생성
            GameRoom gameRoom = new GameRoom();
            gameRoom.Init();

            // 게임룸 추가
            lock(_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }
        public bool Remove(int roomId)
        {
            lock(_lock)
            {
                return _rooms.Remove(roomId);
            }
        }
        public GameRoom Find(int roomId)
        {
            lock(_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;
                return null;
            }
        }
    }
}

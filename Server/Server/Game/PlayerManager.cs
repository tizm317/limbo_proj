using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class PlayerManager
    {
        // 플레이어 관리하는 객체
        public static PlayerManager Instance { get; } = new PlayerManager();
        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 1; // TODO

        // 플레이어 추가(생성), 제거, 찾기
        public Player Add()
        {
            Player player = new Player();
            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }
            return player;
        }
        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }
        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;
                return null;
            }
        }
    }
}

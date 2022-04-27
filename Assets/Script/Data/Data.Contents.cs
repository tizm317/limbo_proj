using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파일 포맷 ( 어떻게 불러읽어들일지) 나타내는 부분 
// Contents용 Data

namespace Data
{
    #region Stat
    [Serializable] // 메모리에서 들고 있는걸 파일로 변환할수있다는 의미
    public class Stat
    {
        // json 파일에서의 이름하고 이 파일에서의 이름 맞춰야함 (다르면 못 찾음)
        // 타입도 맞춰야함
        // public 변수이여야 읽어들임, 아니면 [SerializedField]
        public int level;
        public int hp;
        public int attack;

        // 포맷만 맞춰두고 한번에 불러옴
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        // ILoader 인터페이스 포함

        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            //data.stats.ToDictionary(); // Linq 라는 기능 쓰는데 IOS쪽에서 버그 많아서 잘 안씀
            // 그냥 수동으로 foreach 문으로 넣어줌
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);

            return dict;
        }
    }
    #endregion
    // (추가)
}

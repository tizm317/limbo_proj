using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ( ��� �ҷ��о������) ��Ÿ���� �κ� 
// Contents�� Data

namespace Data
{
    #region Stat
    [Serializable] // �޸𸮿��� ��� �ִ°� ���Ϸ� ��ȯ�Ҽ��ִٴ� �ǹ�
    public class Stat
    {
        // json ���Ͽ����� �̸��ϰ� �� ���Ͽ����� �̸� ������� (�ٸ��� �� ã��)
        // Ÿ�Ե� �������
        // public �����̿��� �о����, �ƴϸ� [SerializedField]
        public int level;
        public int hp;
        public int attack;

        // ���˸� ����ΰ� �ѹ��� �ҷ���
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        // ILoader �������̽� ����

        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            //data.stats.ToDictionary(); // Linq ��� ��� ���µ� IOS�ʿ��� ���� ���Ƽ� �� �Ⱦ�
            // �׳� �������� foreach ������ �־���
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);

            return dict;
        }
    }
    #endregion
    // (�߰�)
}

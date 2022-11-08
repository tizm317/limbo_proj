using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server.DB
{
    [Table("Account")] // 테이블 이름
    public class AccountDb
    {
        // PK (className + Id => PK 셋팅) (Clustered Index)
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }

        // 1 계정 : 다 players
        public ICollection<PlayerDb> Players { get; set; }
    }

    [Table("Player")]
    public class PlayerDb
    {
        // PK
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }
        public int PlayerJob { get; set; }

        // 어떤 Account Id인지 직접 지정하기 위해 foreign key (..?)
        // playerDb 만들 때, AccountDbId를 맘대로 설정해서 save하면 직접 연동할 수 있음
        [ForeignKey("Account")]
        public int AccountDbId { get; set; }
        public AccountDb Account { get; set; } // 소속 계정 정보

        // StatInfo
        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }
    }
}

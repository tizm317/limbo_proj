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

        // 소속 계정 정보
        public AccountDb Account { get; set; }

    }
}

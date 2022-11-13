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

        // 1(player)대 다(Items) 관계
        public ICollection<ItemDb> Items { get; set; }

        // StatInfo
        public int Level { get; set; }
        public float Hp { get; set; }
        public float MaxHp { get; set; }
        public float Attack { get; set; }
        public float Defence { get; set; }
        public float MoveSpeed { get; set; }
        public float TurnSpeed { get; set; }
        public float AttackSpeed { get; set; }

        // PlayerStatInfo
        public int Exp { get; set; }
        public uint Gold { get; set; }
        public float Next_level_up { get; set; }
        public float Current_exp { get; set; }
        public float Regeneration { get; set; }
        public float Mana { get; set; }
        public float Max_mana { get; set; }
        public float Mana_regeneration { get; set; }
        public int Skill_point { get; set; }
    }

    [Table("Item")]
    public class ItemDb
    {
        public int ItemDbId { get; set; } // DB에서 발급한 ID
        public int TemplateId { get; set; } // 우리가 지정한 데이터시트에서의 ID
        public int Count { get; set; } // 들고 있는 아이템 수량
        public int Slot { get; set; }   // 슬롯 인덱스(인벤토리에서 배치시킨 슬롯 위치)
        // 게임에 따라 다르지만 내가 착용한 아이템이나 창고같은 경우도 Slot으로 관리할 수 있음
        // ex) 0~10 : 착용 아이템 / 11~40 : 인벤토리 / 41~100 : 창고 등..

        [ForeignKey("Owner")] // 외래키로 Owner랑 연동
        public int? OwnerDbId { get; set; } // Nullable Type (땅에 떨어졌을 때는 주인 없으니)
        public PlayerDb Owner { get; set; } // 아이템 갖고 있는 주인
    }
}

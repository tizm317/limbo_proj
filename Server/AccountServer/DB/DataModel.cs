using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.DB
{
    [Table("Account")] // 테이블 이름
    public class AccountDb
    {
        // PK (className + Id => PK 셋팅) (Clustered Index)
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
}

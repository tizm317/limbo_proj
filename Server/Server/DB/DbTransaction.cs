using Microsoft.EntityFrameworkCore;
using Server.Game;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
    public class DbTransaction : JobSerializer
    {
        // DB 작업 (GameRoom에서 처리할 부분/Job으로 Db담당한테 보낼 부분 나누는 StepByStep 작업)
        public static DbTransaction Instance { get; } = new DbTransaction();

        // Step 1. Me (GameRoom)
        public static void SavePlayerStatus_Step1(Player player, GameRoom room)
        {
            if (player == null || room == null) return;

            /*  1.DB 읽을 때/ 쓸 때 2번 접근
                PlayerDb playerDb = db.Players.Find(PlayerDbId);
                // TODO: 갱신 내용
                {
                    //playerDb.Hp = StatInfo.Hp;
                }
                db.SaveChanges();
             */

            // 2. DB 한번만 접근하는 방법 (get 없이 바로 update)
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            {
                // TODO : 갱신 내용 추가 ***
                playerDb.Hp = player.StatInfo.Hp;
            }
            Instance.Push<PlayerDb, GameRoom>(SavePlayerStatus_Step2, playerDb, room);
        }

        // Step 2. You (Db)
        public static void SavePlayerStatus_Step2(PlayerDb playerDb, GameRoom room)
        {
            using (AppDbContext db = new AppDbContext())
            {
                // State 조절(EntityFramework 강의)
                db.Entry(playerDb).State = EntityState.Unchanged;
                db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true; // Hp만 변화있는거 감지해서 쿼리 효율적으로 만들어줌
                // TODO: 갱신 내용 추가할 때 Property 수정

                bool success = db.SaveChangesEx();
                if(success == true)
                {
                    room.Push(SavePlayerStatus_Step3, playerDb.Hp);
                }

            }
        }

        // Step 3. Me (GameRoom)
        public static void SavePlayerStatus_Step3(float hp)
        {
            Console.WriteLine($"Hp Saved : {hp}");
        }
    }
}

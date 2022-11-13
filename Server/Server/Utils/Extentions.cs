using Server.DB;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Utils
{
    public static class Extentions
    {
        // SaveChanges 확장
        // Exception Handling : return boolean
        public static bool SaveChangesEx(this AppDbContext db)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveChangesEx(this SharedDbContext db)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

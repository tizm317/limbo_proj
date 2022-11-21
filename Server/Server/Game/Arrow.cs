using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        long _nextMoveTick = 0;

        public override void Update()
        {
            if (Owner == null || Room == null)
                return;
            if (_nextMoveTick >= Environment.TickCount64)
                return;
            _nextMoveTick = Environment.TickCount64 + 50;

        }

    }
}

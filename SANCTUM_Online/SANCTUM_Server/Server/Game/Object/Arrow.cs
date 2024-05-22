using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public Enemy _target;
        long _nextMoveTick = 0;

        public override void Update()
        {
            if (Owner == null || Room == null)
                return;

            if (_nextMoveTick >= Environment.TickCount64)
                return;
            long     tick = (long)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            //if (_target == null || _target.Room != Room)
            //{
            //    _target = null;

            //    // 소멸
            //    Room.LeaveGame(Id);
            //    return;
            //}

            // 타겟에게 도달했다면
            float dist = Vector3.Distance(Pos, _target.Pos);
            if (dist < 1f)
            {
                // 피격
                _target.OnDamaged(Owner.Owner, Stat.Attack);

                // 소멸
                Room.LeaveGame(Id);
                return;
            }

            // 범위에서 벗어났다면 
            dist = Vector3.Distance(Pos, Owner.Pos);
            if (dist > Owner.Range)
            {
                // 소멸
                Room.LeaveGame(Id);
                return;
            }

            // 이동
            Vector3 dir = (_target.Pos - Pos).normalized;
            
            PosInfo.PosX += dir.x;
            PosInfo.PosY += dir.y;
            PosInfo.PosZ += dir.z;
            //Console.WriteLine($"{PosInfo.PosX}, {PosInfo.PosY}, {PosInfo.PosZ}");

            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }
    }
}

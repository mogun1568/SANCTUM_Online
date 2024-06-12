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

            if (Stat.Level == 1)
                ThirdPersonMode();
            else if (Stat.Level == 2)
                FirstPersonMode();
        }

        void ThirdPersonMode()
        {
            if (_nextMoveTick >= Environment.TickCount64)
                return;
            long tick = (long)(1000 / Speed);
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
                HitTarget(_target);
                //_target.OnDamaged(Master, (int)(Attack * Owner.Attack));

                // 소멸
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            // 범위에서 벗어났다면 
            dist = Vector3.Distance(Pos, Owner.Pos);
            if (dist > Owner.Range)
            {
                // 소멸
                Room.Push(Room.LeaveGame, Id);
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

        void FirstPersonMode()
        {
            //if (_nextMoveTick >= Environment.TickCount64)
            //    return;
            //long tick = (long)(1000 / (Speed * 2));
            //_nextMoveTick = Environment.TickCount64 + tick;

            float dist;
            // 타겟에게 도달했다면
            Enemy target = Room.FindEnemy(e =>
            {
                dist = Vector3.Distance(Pos, e.Pos + Center(e.Info.Name));
                return dist <= 2f;
            });

            if (target != null)
            {
                // 피격
                target.OnDamaged(Master, (int)(Attack * Owner.Attack));

                // 소멸
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            // 범위에서 벗어났다면 
            dist = Vector3.Distance(Pos, Owner.Pos);
            if (dist > Owner.Range)
            {
                // 소멸
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            // 이동
            Vector3 dir = Dir.normalized;

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

        Vector3 Center(string name)
        {
            Vector3 v = new Vector3();

            if (name == "SalarymanDefault")
                v.y = 3.5f;
            else
                v.y = 1.5f;

            return v;
        }
    }
}

using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Game
{
    public class Turret : GameObject
    {
        public Turret()
        {
            ObjectType = GameObjectType.Turret;

            // TEMP (타워는 기본 데이터만 존재)
            Stat.Name = "StandardTower";
            Stat.Level = 1;
            Stat.MaxHp = 100;
            Stat.Hp = 100;
            Stat.Attack = 0;
            Stat.Range = 15f;
            Stat.FireRate = 1f;

            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            if (Owner.Owner.Room == null)
            {
                Room.LeaveGame(Id);
                return;
            }

            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Attacking:
                    UpdateAttacking();
                    break;
                case CreatureState.Die:
                    UpdateDie();
                    break;
            }
        }

        Enemy _target;

        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            // 현재는 가장 가까운 위치가 아닌 빨리 생성된 순으로 찾음
            // 가까운 순으로 수정할까 고민 중
            Enemy target = Room.FindEnemy(e =>
            {
                float dist = Vector3.Distance(Pos, e.Pos);
                return dist <= Range;
            });

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Attacking;
        }

        long _nextAttackTick = 0;
        protected virtual void UpdateAttacking()
        {
            if (_nextAttackTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 * FireRate);
            _nextAttackTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            float dist = Vector3.Distance(Pos, _target.Pos);
            if (dist > Range)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            S_Look lookPacket = new S_Look();
            lookPacket.ObjectId = Id;
            lookPacket.TargetPosinfo = _target.PosInfo;
            Room.Broadcast(lookPacket);

            // TODO : 화살 발사
            Console.WriteLine($"Shoot {_target.Id}!");
            Shoot("StandardBullet");
        }

        protected virtual void UpdateDie()
        {

        }

        void Shoot(string name)
        {
            Arrow arrow = ObjectManager.Instance.Add<Arrow>();

            StatInfo projectileInfo = null;
            if (DataManager.ProjectileDict.TryGetValue(name, out projectileInfo) == false)
            {
                return;
            }

            arrow.Info.Name = name;
            arrow.PosInfo.PosX = PosInfo.PosX;
            arrow.PosInfo.PosY = PosInfo.PosY + 5;
            arrow.PosInfo.PosZ = PosInfo.PosZ;

            Vector3 dir = (_target.Pos - arrow.Pos).normalized;
            arrow.PosInfo.DirX = dir.x;
            arrow.PosInfo.DirY = dir.y;
            arrow.PosInfo.DirZ = dir.z;

            arrow.Stat.MergeFrom(projectileInfo);
            arrow.Owner = this;
            arrow._target = _target;

            Room.EnterGame(arrow);
        }
    }
}

using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Game
{
    public class Turret : GameObject
    {
        public StatInfo _projectileInfo;

        public Turret()
        {
            ObjectType = GameObjectType.Turret;

            // TEMP (타워는 기본 데이터만 존재)
            Stat.Name = "StandardTower";
            Stat.Level = 1;
            Stat.MaxHp = 100;
            Stat.Hp = 100;
            Stat.Attack = 1f;
            Stat.Range = 15f;
            Stat.FireRate = 1f;

            State = CreatureState.Idle;

            BulletInfo(Stat.Name);
        }

        public void BulletInfo(string name)
        {
            string s = name;
            if (s == "StandardTower")
                s = "Standard";
            s += "Bullet";

            _projectileInfo = null;
            if (DataManager.ProjectileDict.TryGetValue(s, out _projectileInfo) == false)
            {
                return;
            }
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            Player master = Master as Player;

            if (master.Room == null)
            {
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            if (IsFPM)
            {
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
            // FireRate마다 총알이 나가지 않는 이유는 여기서 return되는 경우(순서를 위로해서 해결)
            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            if (_nextAttackTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 * FireRate);
            _nextAttackTick = Environment.TickCount64 + moveTick;

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

            Console.WriteLine($"Shoot {_target.Id}!");
            // TODO
            Shoot(_projectileInfo.Name);
        }

        protected virtual void UpdateDie()
        {

        }

        void Shoot(string name)
        {
            Arrow arrow = ObjectManager.Instance.Add<Arrow>();

            arrow.Info.Name = name;
            arrow.PosInfo.PosX = PosInfo.PosX;
            arrow.PosInfo.PosY = PosInfo.PosY + 5;
            arrow.PosInfo.PosZ = PosInfo.PosZ;

            Vector3 dir = (_target.Pos - arrow.Pos).normalized;
            arrow.PosInfo.DirX = dir.x;
            arrow.PosInfo.DirY = dir.y;
            arrow.PosInfo.DirZ = dir.z;

            arrow.Stat.MergeFrom(_projectileInfo);
            arrow.Info.StatInfo.Level = 1;
            arrow.Owner = this;
            arrow.Master = Master;
            arrow._target = _target;

            Room.Push(Room.EnterGame, arrow);
        }

        long _nextShootTick = 0;
        public void FPMShoot(PositionInfo pos)
        {
            if (_nextShootTick > Environment.TickCount64)
                return;
            int moveTick = 250;
            _nextShootTick = Environment.TickCount64 + moveTick;

            Arrow arrow = ObjectManager.Instance.Add<Arrow>();

            arrow.Info.Name = "StandardBullet";
            arrow.PosInfo.PosX = pos.PosX;
            arrow.PosInfo.PosY = pos.PosY;
            arrow.PosInfo.PosZ = pos.PosZ;
            arrow.PosInfo.DirX = pos.DirX;
            arrow.PosInfo.DirY = pos.DirY;
            arrow.PosInfo.DirZ = pos.DirZ;

            arrow.Stat.MergeFrom(_projectileInfo);
            arrow.Info.StatInfo.Level = 2;
            arrow.Owner = this;
            arrow.Master = Master;

            Room.Push(Room.EnterGame, arrow);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            Node node = Owner as Node;
            node.DestroyTurret();
        }
    }
}

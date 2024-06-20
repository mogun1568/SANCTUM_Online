using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Enemy : GameObject
    {
        public LinkedListNode<Node> nextRoad;
        Turret _target;

        public Enemy()
        {
            ObjectType = GameObjectType.Enemy;
            State = CreatureState.Moving;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            if (Owner.Room == null)
            {
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            Console.WriteLine(State);

            switch (State)
            {
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Attacking:
                    UpdateAttacking();
                    break;
                case CreatureState.Die:
                    UpdateDie();
                    break;
            }
        }

        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;
            long moveTick = (long)(16000 / (Speed * 4));
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if (Stat.Type != "General")
            {
                if (FindTurret())
                    return;
            }

            if (CanNext(Pos, nextRoad.Value.Pos))
            {
                GetNextRoad();
            }

            if (nextRoad == null)
            {
                _target = null;
                EndPath();
                return;
            }

            // 이동
            PosInfo.PosX = nextRoad.Value.Pos.x;
            PosInfo.PosY = nextRoad.Value.Pos.y;
            PosInfo.PosZ = nextRoad.Value.Pos.z;

            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

        bool FindTurret()
        {
            Turret target = Room.FindTurret(t =>
            {
                float dist = Vector3.Distance(Pos, t.Pos);
                return dist <= Range;
            });

            if (target == null)
                return false;

            _target = target;
            State = CreatureState.Attacking;
            return true;
        }

        bool CanNext(Vector3 pos1, Vector3 pos2)
        {
            float dist = Vector3.Distance(pos1, pos2);

            if (dist < 0.1f)
            {
                return true;
            }

            return false;
        }

        void GetNextRoad()
        {
            nextRoad = nextRoad.Next;
        }

        void EndPath()
        {
            // TODO : 플레어서 life 감소
            Player owner = Owner as Player;
            owner.Stat.Hp -= Exp;

            S_ChangeStat changeStatPacket = new S_ChangeStat();
            changeStatPacket.ObjectId = owner.Id;
            changeStatPacket.StatInfo = owner.Stat;
            owner.Session.Send(changeStatPacket);

            Room.Push(Room.LeaveGame, Id);
        }

        long _nextAttackTick = 0;
        protected virtual void UpdateAttacking()
        {
            if (_nextAttackTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000);
            _nextAttackTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Moving;
                return;
            }

            S_Look lookPacket = new S_Look();
            lookPacket.ObjectId = Id;
            lookPacket.TargetPosinfo = _target.PosInfo;
            Room.Broadcast(lookPacket);

            // 피격
            _target.OnDamaged(Owner, (int)Attack);
        }

        protected virtual void UpdateDie()
        {
            
        }

        public async Task OnDotDamaged(GameObject attacker, int damage)
        {
            OnDamaged(attacker, damage);

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                if (Room == null)
                    return;

                OnDamaged(attacker, damage / 5);
            }
        }

        bool isAlreadSlow;
        public async Task OnSlow(GameObject attacker, int damage)
        {
            OnDamaged(attacker, damage);

            if (isAlreadSlow)
                return;
            isAlreadSlow = true;

            float originalSpeed = Speed;
            Speed *= 0.5f;

            S_ChangeStat changeStatPacket = new S_ChangeStat();
            changeStatPacket.ObjectId = Id;
            changeStatPacket.StatInfo = Stat;
            Room.Broadcast(changeStatPacket);

            await Task.Delay(3000);
            if (Room == null)
                return;

            Speed = originalSpeed;

            changeStatPacket.StatInfo = Stat;
            Room.Broadcast(changeStatPacket);

            isAlreadSlow = false;
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            if (ObjectManager.GetObjectTypeById(Id) == GameObjectType.Enemy)
            {
                Player player = Owner as Player;
                player.GetExp(Exp);
            }
        }
    }
}

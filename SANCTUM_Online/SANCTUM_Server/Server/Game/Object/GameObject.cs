using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        public float Range
        {
            get { return Stat.Range; }
            set { Stat.Range = value; }
        }

        public float FireRate
        {
            get { return Stat.FireRate; }
            set { Stat.FireRate = value; }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        private GameObject owner;
        public GameObject Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                if (owner != null)
                {
                    Info.OwnerId = owner.Id;
                }
            }
        }

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public virtual void Update()
        {

        }

        public Vector3 Pos
        {
            get
            {
                return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
            }

            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
                PosInfo.PosZ = value.z;
            }
        }

        public Vector3 Dir
        {
            get
            {
                return new Vector3(PosInfo.DirX, PosInfo.DirY, PosInfo.DirZ);
            }

            set
            {
                PosInfo.DirX = value.x;
                PosInfo.DirY = value.y;
                PosInfo.DirZ = value.z;
            }
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
            {
                return;
            }

            Stat.Hp = Math.Max(Stat.Hp - damage, 0);
            Console.WriteLine($"Hit ! {Id}, {Stat.Hp}");

            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            Room.Broadcast(changePacket);

            if (Stat.Hp <= 0)
            {
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            Room.LeaveGame(Id);
        }
    }
}

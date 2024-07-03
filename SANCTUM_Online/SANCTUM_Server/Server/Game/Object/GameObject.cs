﻿using Google.Protobuf.Protocol;
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
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Attack
        {
            get { return Stat.Attack; }
            set { Stat.Attack = value; }
        }

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

        public int Exp
        {
            get { return Stat.Exp; }
            set { Stat.Exp = value; }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        private GameObject master;
        public GameObject Master
        {
            get { return master; }
            set
            {
                master = value;
                if (master != null)
                {
                    Info.MasterId = master.Id;
                }

                if (owner == null)
                {
                    Owner = master;
                }
            }
        }

        private GameObject owner;
        public GameObject Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                //if (owner != null)
                //{
                //    Info.OwnerId = owner.Id;
                //}
            }
        }

        public bool IsFPM
        {
            get { return Stat.IsFPM; }
            set { Stat.IsFPM = value; }
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

        public void CopyStat(StatInfo source, StatInfo target)
        {
            target.Name = source.Name;
            target.Type = source.Type;
            target.HaveEnvironment = source.HaveEnvironment;
            target.Level = source.Level;
            target.MaxHp = source.MaxHp;
            target.Hp = source.Hp;
            target.Attack = source.Attack;
            target.StartSpeed = source.StartSpeed;
            target.Speed = source.Speed;
            target.Range = source.Range;
            target.FireRate = source.FireRate;
            target.Exp = source.Exp;
            target.TotalExp = source.TotalExp;
            target.IsFPM = source.IsFPM;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;

            if (State == CreatureState.Die || Stat.Hp <= 0)
                return;

            Stat.Hp = Math.Max(Stat.Hp - damage, 0);
            Console.WriteLine($"Hit ! {Id}, {Stat.Hp}");

            S_ChangeStat changeStatPacket = new S_ChangeStat();
            changeStatPacket.ObjectId = Id;
            changeStatPacket.StatInfo = Stat;
            if (ObjectType == GameObjectType.Player)
                changeStatPacket.ChangeStat = ChangeStat.UserLifeDown;
            Room.Broadcast(changeStatPacket);

            if (Stat.Hp <= 0)
            {
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            State = CreatureState.Die;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            if (ObjectManager.GetObjectTypeById(Id) == GameObjectType.Enemy)
                Room.PushAfter(3000, Room.LeaveGame, Id);
            else if (ObjectManager.GetObjectTypeById(Id) != GameObjectType.Player)
                Room.Push(Room.LeaveGame, Id);
        }
    }
}

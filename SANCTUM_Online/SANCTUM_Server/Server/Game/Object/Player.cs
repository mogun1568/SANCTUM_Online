using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public Map Map { get; private set; } = new Map();
        public Inventory Inventory { get; private set; } = new Inventory();

        public void Init(int mapid)
		{
            SetMapStartPoint();
            Map.Init(this);
            Inventory.Init(Id, Room);
        }

        int _countLevelUp = 0;
        public void GetExp(int exp)
        {
            Exp += exp;

            while (Exp >= Stat.TotalExp)
            {
                Exp -= Stat.TotalExp;
                Stat.TotalExp = (int)(Stat.TotalExp * 1.5f);
                Math.Min(Stat.TotalExp, 10);
                _countLevelUp++;
            }

            S_ExpUpdate expUpdatePacket = new S_ExpUpdate();
            expUpdatePacket.PlayerId = Id;
            expUpdatePacket.Exp = Exp;
            expUpdatePacket.TotalExp = Stat.TotalExp;
            expUpdatePacket.CountLevelUp = _countLevelUp;
            Session.Send(expUpdatePacket);

            _countLevelUp = 0;
        }

        public void SetMapStartPoint()
        {
            Map.startR = (int)PosInfo.PosX;
            Map.startC = (int)PosInfo.PosZ;
        }

        public void ExpendMap()
        {
            Map.ExpendMap();
        }

		public List<Node> nodeInfos()
		{
			return Map.nodes;
		}

        Random random = new Random();
        public string EnemyName()
        {
            int idx = random.Next(0, DataManager.EnemyList.Count);
            while (DataManager.EnemyList[idx] == "SalarymanDefault")
            {
                idx = random.Next(0, DataManager.EnemyList.Count);
            }

            return DataManager.EnemyList[idx];
        }

		public void ClearNodes()
		{
            Map.nodes.Clear();
		}
    }
}

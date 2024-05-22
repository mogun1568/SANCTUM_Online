using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
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

        public void Init(int mapid)
		{
            SetMapStartPoint();
            Map.Init(this);		
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

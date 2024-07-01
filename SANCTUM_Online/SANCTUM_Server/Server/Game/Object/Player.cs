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

        public WaitingRoom WaitingRoom { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public Map Map { get; private set; } = new Map();
        public Inventory Inventory { get; private set; } = new Inventory();
        public LevelManager LevelManager { get; private set; } = new LevelManager();

        //public int isFPM;

        public void Init(int mapid)
        {
            SetMapStartPoint();
            Map.Init(this);
            Inventory.Init(this, Room);
            LevelManager.Init(this);
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
            int idx = random.Next(0, DataManager.EnemyDict.Count);
            while (DataManager.EnemyDict.Values.ElementAt(idx).Name == "SalarymanDefault")
            {
                idx = random.Next(0, DataManager.EnemyDict.Count);
            }

            return DataManager.EnemyDict.Values.ElementAt(idx).Name;
        }

        public void ClearNodes()
        {
            Map.nodes.Clear();
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            // TODO : 게임 종료 패킷 전송
            Console.WriteLine($"{Id} is Dead");

            // 유저 사망 시 유저의 맵에서 일어나는 현상들 다 정지
            Map = null;

            S_GameOver gameOverPacket = new S_GameOver();
            gameOverPacket.PlayerId = Id;
            gameOverPacket.Rank = MyRank();
            Session.Send(gameOverPacket);
        }

        int MyRank()
        {
            int rank = Room._players.Count;

            foreach (Player player in Room._players.Values)
            {
                if (player == this)
                    continue;

                if (player.State == CreatureState.Die)
                    rank--;
            }

            return rank;
        }
    }
}

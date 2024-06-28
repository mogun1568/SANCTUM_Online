using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Inventory
    {
        public GameRoom Room { get; set; }

        Dictionary<string, int> _invenDict = new Dictionary<string, int>();
        List<string> _items = new List<string>();
        Player _master;

        #region INVENTORY_EDITOR

        void GenerateInventory()
        {
            //GenerateByPath("../../../../../Common/MapData");
            GenerateByItem("../../../../../SANCTUM_Client/Assets/Resources/Inventory");
        }

        void GenerateByItem(string pathPrefix)
        {
            using (var writer = File.CreateText($"{pathPrefix}/Inventory_{_master.Id}.txt"))
            {
                writer.WriteLine(_invenDict.Count);

                foreach (string item in _items)
                {
                    writer.WriteLine(item);
                    writer.WriteLine(_invenDict[item]);
                }
            }
        }

        #endregion

        public void Init(Player player, GameRoom room)
        {
            _master = player;
            Room = room;

            for (int i = 0; i < 5; i++)
            {
                AddItem("StandardTower");
                AddItem("Water");
                AddItem("DamageUp");
                AddItem("RangeUp");
                AddItem("LifeRecovery");
            }

            GenerateInventory();
        }

        public void AddItem(string itemName)
        {
            if (_invenDict.ContainsKey(itemName))
            {
                // 이미 해당 아이템이 인벤토리에 있는 경우
                _invenDict[itemName]++; // 아이템 개수를 1 증가시킴
            }
            else
            {
                // 새로운 아이템을 획득한 경우
                _invenDict.Add(itemName, 1); // 아이템을 딕셔너리에 추가하고 개수를 1로 초기화함
                _items.Add(itemName);
            }

            GenerateInventory();
        }

        public void UseItem(string itemName, int nodeId, PositionInfo pos)
        {
            ItemInfo itemInfo = null;
            if (DataManager.ItemDict.TryGetValue(itemName, out itemInfo) == false)
            {
                Console.WriteLine("itemInfo is null");
                return;
            }

            if (itemInfo.ItemType != "WorldOnlyItem")
            {
                // TODO : Find가 문제가 될지 안될지 모름
                Node node = Room.Find<Node>(nodeId);
                if (node == null)
                    return;

                if (!node.CanUseItem(itemInfo, pos))
                {
                    Console.WriteLine("Can't use item");
                    return;
                }
            }
            else
            {
                switch (itemInfo.ItemName)
                {
                    case "LifeRecovery":
                        Console.WriteLine($"Life Recovery {_master.Stat.Hp} -> {_master.Stat.Hp + 1}");
                        _master.Stat.Hp += 1;
                        break;
                }

                S_ChangeStat changeStatPacket = new S_ChangeStat();
                changeStatPacket.ObjectId = _master.Id;
                changeStatPacket.StatInfo = _master.Stat;
                changeStatPacket.IsItem = false;    // 따로 효과음이 있기 때문
                changeStatPacket.ChangeStat = ChangeStat.UserLifeUp;
                Room.Broadcast(changeStatPacket);
            }

            _invenDict[itemName]--;
            if (_invenDict[itemName] == 0)
            {
                _invenDict.Remove(itemName);
                _items.Remove(itemName);
            }

            GenerateInventory();
        }
    }
}

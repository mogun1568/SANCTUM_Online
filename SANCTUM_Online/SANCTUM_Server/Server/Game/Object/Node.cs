using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Game
{
    public class Node : GameObject
    {
        public Node()
        {
            ObjectType = GameObjectType.Node;
        }

        public GameObject _turret;
        public bool _haveEnvironment;
        string _element;         // 적용된 원소
        int _upgradedNum = 0;    // 원소 적용 (3번까지 가능)
        int _countItem = 0;      // 노드 위에 사용된 아이템 수

        public bool CanUseItem(string itemName, PositionInfo pos)
        {
            ItemInfo itemInfo = null;
            if (DataManager.ItemDict.TryGetValue(itemName, out itemInfo) == false)
            {
                Console.WriteLine("itemInfo is null");
                return false;
            }   

            switch (itemInfo.ItemType)
            {
                case "Tower":
                    if (_turret != null || _haveEnvironment)
                        return false;
                    BuildTurret(itemName, pos);
                    break;
                case "Element":
                    if (_turret == null)
                        return false;
                    ApplicateElement(itemName, pos);
                    break;
                case "TowerOnlyItem":
                    if (_turret == null)
                        return false;
                    UseTowerOnlyItem(itemName, itemInfo);
                    break;
                case "WorldOnlyItem":
                    UseWolrdOnlyItem();
                    break;
            }

            _countItem++;
            return true;
        }

        void BuildTurret(string itemName, PositionInfo pos)
        {
            Player player = Owner as Player;
            if (player == null)
                return;

            Turret turret = ObjectManager.Instance.Add<Turret>();
            if (turret == null)
                return;

            turret.Info.Name = $"{itemName}lvl0{_upgradedNum}";
            turret.Owner = this;
            turret.Master = Master;
            turret.PosInfo.PosX = pos.PosX;
            turret.PosInfo.PosY = pos.PosY;
            turret.PosInfo.PosZ = pos.PosZ;

            _turret = turret;

            Room.Push(Room.EnterGame, turret);

            //{
            //    Console.WriteLine(_turret.Stat.Name);
            //    Console.WriteLine(_turret.Stat.Level);
            //    Console.WriteLine(_turret.Stat.MaxHp);
            //    Console.WriteLine(_turret.Stat.Hp);
            //    Console.WriteLine(_turret.Stat.Attack);
            //    Console.WriteLine(_turret.Stat.Range);
            //    Console.WriteLine(_turret.Stat.FireRate);
            //}
        }

        void ApplicateElement(string itemName, PositionInfo pos)
        {
            if (_upgradedNum > 0 && _element != itemName)
            {
                Console.WriteLine("already ues element!");
                return;
            }
            _element = itemName;

            if (_upgradedNum >= 3)
            {
                Console.WriteLine("Upgrade Done!");
                return;
            }

            Console.WriteLine($"{itemName} Upgrade {_upgradedNum} -> {_upgradedNum + 1}");
            _upgradedNum++;

            // 원소 타워 생성
            Turret turret = ObjectManager.Instance.Add<Turret>();
            if (turret == null)
                return;

            turret.Info.Name = $"{itemName}Towerlvl0{_upgradedNum}";
            turret.Owner = this;
            turret.Master = Master;
            turret.PosInfo.PosX = pos.PosX;
            turret.PosInfo.PosY = pos.PosY;
            turret.PosInfo.PosZ = pos.PosZ;
            turret.CopyStat(_turret.Stat, turret.Stat);
            turret.Stat.Name = itemName;
            turret.BulletInfo(itemName);

            Room.Push(Room.LeaveGame, _turret.Id);
            Room.Push(Room.EnterGame, turret);
            _turret = turret;
        }

        void UseTowerOnlyItem(string itemName, ItemInfo itemInfo)
        {
            switch (itemName)
            {
                case "DamageUp":
                    Console.WriteLine($"Damage Up {_turret.Attack} -> {_turret.Attack * itemInfo.UpgradeAmount}");
                    _turret.Attack *= itemInfo.UpgradeAmount;
                    break;
                case "RangeUp":
                    Console.WriteLine($"Range Up {_turret.Range} -> {_turret.Range * itemInfo.UpgradeAmount}");
                    _turret.Range *= itemInfo.UpgradeAmount;
                    break;
                case "FireRateUp":
                    Console.WriteLine($"Range Up {_turret.FireRate} -> {_turret.FireRate * itemInfo.UpgradeAmount}");
                    _turret.FireRate *= itemInfo.UpgradeAmount;
                    break;
            }

            S_ChangeStat changeStatPacket = new S_ChangeStat();
            changeStatPacket.ObjectId = _turret.Id;
            changeStatPacket.StatInfo = _turret.Stat;
            changeStatPacket.IsItem = true;
            Room.Broadcast(changeStatPacket);

            Console.WriteLine(_turret.Stat);
        }

        void UseWolrdOnlyItem()
        {

        }

        public void DemoliteTurret()
        {
            Player player = Owner as Player;
            player.GetExp((int)(_countItem * 1.5));

            Room.Push(Room.LeaveGame, _turret.Id);
            DestroyTurret();
        }

        public void DestroyTurret()
        {
            _turret = null;
            _element = null;
            _upgradedNum = 0;
            _countItem = 0;
        }
    }
}

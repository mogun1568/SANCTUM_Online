using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
                    ApplicateElement();
                    break;
                case "TowerOnlyItem":
                    if (_turret == null)
                        return false;
                    UseTowerOnlyItem();
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

            turret.Info.Name = itemName;
            turret.Owner = this;
            turret.PosInfo.PosX = pos.PosX;
            turret.PosInfo.PosY = pos.PosY;
            turret.PosInfo.PosZ = pos.PosZ;

            _turret = turret;

            Room.Push(Room.EnterGame, turret);
        }

        void ApplicateElement()
        {
            
        }

        void UseTowerOnlyItem()
        {
            
        }

        void UseWolrdOnlyItem()
        {

        }
    }
}

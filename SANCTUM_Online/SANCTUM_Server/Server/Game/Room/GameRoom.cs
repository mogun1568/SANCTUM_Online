using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Server.Game
{
    public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }

        public Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
        Dictionary<int, Turret> _turrets = new Dictionary<int, Turret>();
        Dictionary<int, Enemy> _enemys = new Dictionary<int, Enemy>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Wave Wave { get; private set; } = new Wave();

        bool isStart;

        public void Init()
        {
            Wave.Room = this;
        }

        // 누군가가 주기적으로 호출해줘야 한다
        public void Update()
        {
            //if (_players.Count < 1)
            //{
            //    return;
            //}

            Flush();

            if (!isStart)
            {
                return;
            }

            Wave.Update();

            foreach (Enemy enemy in _enemys.Values)
            {
                enemy.Update();
            }

            foreach (Turret turret in _turrets.Values)
            {
                turret.Update();
            }

            foreach (Projectile projectile in _projectiles.Values)
            {
                projectile.Update();
            }
        }

        public void EnterGame(GameObject gameObject)
		{
			if (gameObject == null)
				return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
                Console.WriteLine(player.Id);

                // 아직 조금 더 수정 필요
                //Console.WriteLine(_players.Count);
                if (_players.Count == 0)
                {
                    player.Info.PosInfo.PosX = -100;
                    player.Info.PosInfo.PosZ = -100;
                }
                else if (_players.Count == 1)
                {
                    player.Info.PosInfo.PosX = 0;
                    player.Info.PosInfo.PosZ = -100;
                }
                else if (_players.Count == 2)
                {
                    player.Info.PosInfo.PosX = -100;
                    player.Info.PosInfo.PosZ = 0;
                }
                else
                {
                    player.Info.PosInfo.PosX = 0;
                    player.Info.PosInfo.PosZ = 0;
                }

                _players.Add(gameObject.Id, player);
                player.Room = this;
                player.Init(player.Id);

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);

                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    }

                    foreach (Node n in _nodes.Values)
                    {
                        spawnPacket.Objects.Add(n.Info);
                    }

                    foreach (Turret t in _turrets.Values)
                    {
                        spawnPacket.Objects.Add(t.Info);
                    }

                    foreach (Enemy e in _enemys.Values)
                    {
                        spawnPacket.Objects.Add(e.Info);
                    }

                    foreach (Projectile p in _projectiles.Values)
                    {
                        spawnPacket.Objects.Add(p.Info);
                    }

                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Node)
            {
                Node node = gameObject as Node;
                _nodes.Add(gameObject.Id, node);
                node.Room = this;
            }
            else if (type == GameObjectType.Turret)
            {
                Turret turret = gameObject as Turret;
                _turrets.Add(gameObject.Id, turret);
                turret.Room = this;
            }
            else if (type == GameObjectType.Enemy)
            {
                Enemy enemy = gameObject as Enemy;
                _enemys.Add(gameObject.Id, enemy);
                enemy.Room = this;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }

            // 타인한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
                }
            }
        }

        public void LeaveGame(int objectId)
		{
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                {
                    return;
                }

                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Node)
            {
                Node node = null;
                if (_nodes.Remove(objectId, out node) == false)
                {
                    return;
                }

                node.Room = null;
            }
            else if (type == GameObjectType.Turret)
            {
                Turret turret = null;
                if (_turrets.Remove(objectId, out turret) == false)
                {
                    return;
                }

                turret.Room = null;
            }
            else if (type == GameObjectType.Enemy)
            {
                Enemy enemy = null;
                if (_enemys.Remove(objectId, out enemy) == false)
                {
                    return;
                }

                enemy.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                {
                    return;
                }

                projectile.Room = null;
            }

            // 타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
                }
            }
        }

        public void HandleGameStart(Player player, C_GameStart gameStartPacket)
        {
            if (player == null)
            {
                return;
            }

            foreach (Player p in _players.Values)
            {
                foreach (Node node in p.nodeInfos())
                {
                    node.Owner = p;
                    Push(EnterGame, node);
                }

                S_CreateMap rescreateMapPacket = new S_CreateMap();
                rescreateMapPacket.PlayerId = p.Id;
                Broadcast(rescreateMapPacket);
            }

            // 다른 플레이어한테도 알려준다
            S_GameStart resGameStartPacket = new S_GameStart();
            resGameStartPacket.PlayerId = player.Id;
            Broadcast(resGameStartPacket);

            isStart = true;
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
            {
                return;
            }

            // TODO : 검증

            // 다른 플레이어한테도 알려준다
        }

        public void HandleInvenUpdate(Player player, C_InvenUpdate invenUpdatePacket)
        {
            if (player == null)
            {
                return;
            }

            // TODO : 검증

            if (invenUpdatePacket.IsAdd)
            {
                player.Inventory.AddItem(invenUpdatePacket.ItemName);
            }
            else
            {
                player.Inventory.UseItem(invenUpdatePacket.ItemName, invenUpdatePacket.NodeId, invenUpdatePacket.PosInfo);
            }

            S_InvenUpdate resInvenUpdatePacket = new S_InvenUpdate();
            resInvenUpdatePacket.PlayerId = player.Id;
            player.Session.Send(resInvenUpdatePacket);
        }

        public void HandleTurretUI(Player player, C_TurretUI turretUIPacket)
        {
            if (player == null)
            {
                return;
            }

            // TODO : 검증

            Node node = Find<Node>(turretUIPacket.NodeId);
            if (node._turret == null)
                return;

            S_TurretUI resturretUIPacket = new S_TurretUI();
            resturretUIPacket.PlayerId = player.Id;
            resturretUIPacket.NodeId = turretUIPacket.NodeId;
            resturretUIPacket.StatInfo = node._turret.Stat;
            player.Session.Send(resturretUIPacket);
        }

        public T Find<T>(int objectId) where T : class
        {
            GameObjectType objectType = ObjectManager.GetObjectTypeById(objectId);
            T obj = null;

            if (objectType == GameObjectType.Node)
            {
                Node node = null;
                if (_nodes.TryGetValue(objectId, out node))
                    obj = node as T;
            } else if (objectType == GameObjectType.Turret)
            {
                Turret turret = null;
                if (_turrets.TryGetValue(objectId, out turret))
                    obj = turret as T;
            }

            return obj;
        }

        public Enemy FindEnemy(Func<GameObject, bool> condition)
        {
            foreach (Enemy enemy in _enemys.Values)
            {
                if (condition.Invoke(enemy))
                    return enemy;
            }

            return null;
        }

        public Turret FindTurret(Func<GameObject, bool> condition)
        {
            foreach (Turret turret in _turrets.Values)
            {
                if (condition.Invoke(turret))
                    return turret;
            }

            return null;
        }

        public void Broadcast(IMessage packet)
        {
            // 매번 lock을 거는 것은 멀티쓰레드의 의미가 희석됨
            // 고로 Job을 통해 단일쓰레드에서 쭉 실행하는 방식 이용(완료)
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
    } 
}

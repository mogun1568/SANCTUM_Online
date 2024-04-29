using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class GameRoom
	{
		object _lock = new object();
		public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public void EnterGame(Player newPlayer)
		{
			if (newPlayer == null)
				return;

			lock (_lock)
			{
                // 아직 조금 더 수정 필요
                Console.WriteLine(_players.Count);
                if (_players.Count == 0)
				{
                    newPlayer.Info.PosInfo.PosX = -100;
                    newPlayer.Info.PosInfo.PosZ = -100;
				}
				else if (_players.Count == 1)
				{
                    newPlayer.Info.PosInfo.PosX = 0;
                    newPlayer.Info.PosInfo.PosZ = -100;
				}
				else if (_players.Count == 2)
				{
                    newPlayer.Info.PosInfo.PosX = -100;
                    newPlayer.Info.PosInfo.PosZ = 0;
				}
				else
				{
                    newPlayer.Info.PosInfo.PosX = 0;
                    newPlayer.Info.PosInfo.PosZ = 0;
				}

				_players.Add(newPlayer.Info.PlayerId, newPlayer);
				newPlayer.Room = this;

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = newPlayer.Info;
					newPlayer.Session.Send(enterPacket);

					S_Spawn spawnPacket = new S_Spawn();
					foreach (Player p in _players.Values)
					{
						if (newPlayer != p)
							spawnPacket.Players.Add(p.Info);
					}
					newPlayer.Session.Send(spawnPacket);
				}

				// 타인한테 정보 전송
				{
					S_Spawn spawnPacket = new S_Spawn();
					spawnPacket.Players.Add(newPlayer.Info);
					foreach (Player p in _players.Values)
					{
						if (newPlayer != p)
							p.Session.Send(spawnPacket);
					}
				}

                newPlayer.Init(newPlayer.Info.PlayerId);
            }
		}

		public void LeaveGame(int playerId)
		{
			lock (_lock)
			{
				Player player = null;
				if (_players.Remove(playerId, out player)== false)
                {
                    return;
                }

				player.Room = null;

                // 본인한테 정보 전송
                {
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}

				// 타인한테 정보 전송
				{
					S_Despawn despawnPacket = new S_Despawn();
					despawnPacket.PlayerIds.Add(player.Info.PlayerId);
					foreach (Player p in _players.Values)
					{
						if (player != p)
							p.Session.Send(despawnPacket);
					}
				}
			}
		}

        public void HandleGameStart(Player player, C_GameStart gameStartPacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                // 다른 플레이어한테도 알려준다
                S_GameStart resGameStartPacket = new S_GameStart();
                resGameStartPacket.PlayerId = player.Info.PlayerId;
                player.Room.Broadcast(resGameStartPacket);
            }
        }

        public void HandleCreateMap(Player player, C_CreateMap createMapPacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                // 다른 플레이어한테도 알려준다
                if (createMapPacket.IsStart)
                {
                    foreach (Player p in _players.Values)
                    {
                        S_CreateMap rescreateMapPacket = new S_CreateMap();
                        rescreateMapPacket.PlayerId = p.Info.PlayerId;
                        foreach (NodeInfo nodeInfo in p.nodeInfos())
                        {
                            rescreateMapPacket.NodeInfo.Add(nodeInfo);
                        }

                        p.Room.Broadcast(rescreateMapPacket);
                    }
                }
                else
                {
                    player.ExpendMap();
                    S_CreateMap rescreateMapPacket = new S_CreateMap();
                    rescreateMapPacket.PlayerId = player.Info.PlayerId;
                    foreach (NodeInfo nodeInfo in player.nodeInfos())
                    {
                        rescreateMapPacket.NodeInfo.Add(nodeInfo);
                    }

                    player.Room.Broadcast(rescreateMapPacket);
                }
            }
        }

        public void HandleSpawnEnemy(Player player, C_SpawnEnemy spawnEnemyPacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                // 다른 플레이어한테도 알려준다
                S_SpawnEnemy resSpawnEnemyPacket = new S_SpawnEnemy();
                resSpawnEnemyPacket.PlayerId = player.Info.PlayerId;
                resSpawnEnemyPacket.EnemyName = spawnEnemyPacket.EnemyName;

                player.Room.Broadcast(resSpawnEnemyPacket);
            }
        }

        
        public void HandleCreateTurret(Player player, C_CreateTurret createTurretPacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                // 다른 플레이어한테도 알려준다
                S_CreateTurret rescreateTurretPacket = new S_CreateTurret();
                rescreateTurretPacket.PlayerId = player.Info.PlayerId;
                rescreateTurretPacket.NodeId = createTurretPacket.NodeId;
                rescreateTurretPacket.ItemName = createTurretPacket.ItemName;

                player.Room.Broadcast(rescreateTurretPacket);
            }
        }

        public void Broadcast(IMessage packet)
        {
            // 매번 lock을 거는 것은 멀티쓰레드의 의미가 희석됨
            // 고로 Job을 통해 단일쓰레드에서 쭉 실행하는 방식 이용(예정)
            lock (_lock)
            {
                foreach (Player p in _players.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }
    } 
}

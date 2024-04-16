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

        public List<Player> _players = new List<Player>();

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

				_players.Add(newPlayer);
				newPlayer.Room = this;

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = newPlayer.Info;
					newPlayer.Session.Send(enterPacket);

					S_Spawn spawnPacket = new S_Spawn();
					foreach (Player p in _players)
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
					foreach (Player p in _players)
					{
						if (newPlayer != p)
							p.Session.Send(spawnPacket);
					}
				}
			}
		}

		public void LeaveGame(int playerId)
		{
			lock (_lock)
			{
				Player player = _players.Find(p => p.Info.PlayerId == playerId);
				if (player == null)
					return;

				_players.Remove(player);
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
					foreach (Player p in _players)
					{
						if (player != p)
							p.Session.Send(despawnPacket);
					}
				}
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
                S_CreateMap rescreateMapPacket = new S_CreateMap();
                rescreateMapPacket.PlayerId = player.Info.PlayerId;
                foreach (NodeInfo nodeInfo in createMapPacket.NodeInfo)
                {
                    rescreateMapPacket.NodeInfo.Add(nodeInfo);
                }

                player.Room.Broadcast(rescreateMapPacket);
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
            {
                return;
            }

            lock (_lock)
            {
                // TODO : 검증

                // 일단 서버에서 좌표 이동
                // C#의 클래스는 C++과 다르게 값 타입이 아니라 참조 타입임
                PlayerInfo info = player.Info;
                info.PosInfo = movePacket.PosInfo;

                // 다른 플레이어한테도 알려준다
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = info.PlayerId;
                resMovePacket.IsStart = movePacket.IsStart;
                resMovePacket.PosInfo = movePacket.PosInfo;

                player.Room.Broadcast(resMovePacket);
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

        public void Broadcast(IMessage packet)
        {
            // 매번 lock을 거는 것은 멀티쓰레드의 의미가 희석됨
            // 고로 Job을 통해 단일쓰레드에서 쭉 실행하는 방식 이용(예정)
            lock (_lock)
            {
                foreach (Player p in _players)
                {
                    p.Session.Send(packet);
                }
            }
        }
    } 
}

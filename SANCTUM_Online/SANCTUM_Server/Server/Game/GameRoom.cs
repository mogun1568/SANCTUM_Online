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

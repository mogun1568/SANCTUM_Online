using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Player
	{
		public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
		public GameRoom Room { get; set; }
		public ClientSession Session { get; set; }

		Map _map = new Map();

		public void Init(int mapid)
		{
			_map.Init(mapid);
		}

        public void ExpendMap()
        {
            _map.ExpendMap();
        }

		public List<NodeInfo> nodeInfos()
		{
			return _map.nodes;
		}

		public void ClearNodes()
		{
			_map.nodes.Clear();
		}
    }
}

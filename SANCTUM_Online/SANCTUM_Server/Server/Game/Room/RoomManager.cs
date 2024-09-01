using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class RoomManager
	{
		public static RoomManager Instance { get; } = new RoomManager();

		object _lock = new object();
        Dictionary<int, WaitingRoom> _waitingRooms = new Dictionary<int, WaitingRoom>();
        public Dictionary<int, GameRoom> _gameRooms = new Dictionary<int, GameRoom>();
		int _waitingRoomId = 1, _gameRoomId = 1;

        public WaitingRoom AddWaitingRoom()
        {
            WaitingRoom waitingRoom = new WaitingRoom();
            waitingRoom.Push(waitingRoom.Init);

            lock (_lock)
            {
                waitingRoom.RoomId = _waitingRoomId;
                _waitingRooms.Add(_waitingRoomId, waitingRoom);
                _waitingRoomId++;
            }

            Program.TickWaitingRoom(waitingRoom, 50);

            return waitingRoom;
        }

        public bool RemoveWaitingRoom(int roomId)
        {
            lock (_lock)
            {
                return _waitingRooms.Remove(roomId);
            }
        }

        public GameRoom AddGameRoom()
		{
			GameRoom gameRoom = new GameRoom();
			gameRoom.Push(gameRoom.Init);

			lock (_lock)
			{
				gameRoom.RoomId = _gameRoomId;
				_gameRooms.Add(_gameRoomId, gameRoom);
                _gameRoomId++;
			}

            Program.TickRoom(gameRoom, 50);

            return gameRoom;
		}

		public bool RemoveGameRoom(int roomId)
		{
			lock (_lock)
			{
                Console.WriteLine($"Remove GameRoom {roomId}");
				return _gameRooms.Remove(roomId);
			}
		}

        public WaitingRoom FindWaitingRoom(int roomId)
        {
            lock (_lock)
            {
                WaitingRoom room = null;
                if (_waitingRooms.TryGetValue(roomId, out room))
                    return room;

                return null;

                //if (roomId == 0)
                //{
                //    room = _waitingRooms.Values.FirstOrDefault(room => room._players.Count < 4);
                //}
                //else
                //{
                //    if (_waitingRooms.TryGetValue(roomId, out room))
                //        return room;
                //}

                //if (room == null)
                //{
                //    room = AddWaitingRoom();
                //}

                //return room;
            }
        }

        public GameRoom FindGameRoom(int roomId)
        {
            lock (_lock)
            {
                GameRoom room = null;
                if (_gameRooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }
    }
}

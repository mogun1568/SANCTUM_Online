using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Game
{
    public class WaitingRoom : JobSerializer
    {
        public int RoomId { get; set; }

        public Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public void Init()
        {
            
        }

        public void Update()
        {
            Flush();
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            Player player = gameObject as Player;
            Console.WriteLine(player.Id);

            _players.Add(gameObject.Id, player);
            player.WaitingRoom = this;

            S_EnterGame enterPacket = new S_EnterGame();
            enterPacket.Player = player.Info;
            enterPacket.IsGameRoom = false;
            player.Session.Send(enterPacket);

            S_RoomList roomListPacket = new S_RoomList();
            foreach (GameRoom gr in RoomManager.Instance._gameRooms.Values)
            {
                RoomInfo roomInfo = new RoomInfo();
                roomInfo.Id = gr.RoomId;
                roomInfo.PlayerNum = gr._players.Count;

                roomListPacket.RoomList.Add(roomInfo);
            }
            player.Session.Send(roomListPacket);

            // WaitingRoom에서는 클라에서 따로 들어가도 보여주는 것이 없기 때문에
            // 자신에서 다른 플레이어를 보여주거나 반대도 따로 패킷을 보내주지 않아도 됨
        }

        public void LeaveGame(int objectId)
        {
            Player player = null;
            if (_players.Remove(objectId, out player) == false)
            {
                return;
            }

            player.WaitingRoom = null;

            // 본인한테 정보 전송
            {
                S_LeaveGame leavePacket = new S_LeaveGame();
                player.Session.Send(leavePacket);
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

        public void HandleRoomList(Player player, C_RoomList roomListPacket)
        {
            if (player == null)
            {
                return;
            }

            S_RoomList resRoomListPacket = new S_RoomList();
            foreach (GameRoom gr in RoomManager.Instance._gameRooms.Values)
            {
                RoomInfo roomInfo = new RoomInfo();
                roomInfo.Id = gr.RoomId;
                roomInfo.PlayerNum = gr._players.Count;

                resRoomListPacket.RoomList.Add(roomInfo);
            }
            player.Session.Send(resRoomListPacket);
        }

        public void HandleEnterRoom(Player player, C_EnterRoom enterRoomPacket)
        {
            if (player == null)
            {
                return;
            }

            GameRoom room;
            if (enterRoomPacket.RoomId == default)
            {
                // 새 GameRoom 생성
                room = RoomManager.Instance.AddGameRoom();
            }
            else
            {
                // GameRoom 찾기
                room = RoomManager.Instance.FindGameRoom(enterRoomPacket.RoomId);
            }
            room.Push(room.EnterGame, player);

            Push(LeaveGame, player.Id);
        }
    }
}

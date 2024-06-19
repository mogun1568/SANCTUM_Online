using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using System.Numerics;
using Server.Data;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public Player MyPlayer { get; set; }
        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // 원래는 서버에서 패킷을 보내고 클라에서 준비됐다는 패킷을 보내면 입장시킴
            // 지금은 그냥 강제 입장시키는 방식으로 진행
            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;
                MyPlayer.Info.PosInfo.PosZ = 0;

                MyPlayer.Stat.Level = 1;
                MyPlayer.Stat.MaxHp = 10;
                MyPlayer.Stat.Hp = 10;
                MyPlayer.Stat.Exp = 0;
                MyPlayer.Stat.TotalExp = 3;

                // TODO
                //StatInfo stat = null;
                //DataManager.StatDict.TryGetValue(1, out stat);
                //MyPlayer.Stat.MergeFrom(stat);

                MyPlayer.Session = this;
            }

            WaitingRoom room = RoomManager.Instance.FindWaitingRoom(1);
            room.Push(room.EnterGame, MyPlayer);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            if (MyPlayer.Room != null)
            {
                GameRoom room = RoomManager.Instance.FindGameRoom(MyPlayer.Room.RoomId);
                room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);
            }
            else
            {
                WaitingRoom waitingRoom = RoomManager.Instance.FindWaitingRoom(MyPlayer.WaitingRoom.RoomId);
                waitingRoom.Push(waitingRoom.LeaveGame, MyPlayer.Info.ObjectId);
            }

            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}

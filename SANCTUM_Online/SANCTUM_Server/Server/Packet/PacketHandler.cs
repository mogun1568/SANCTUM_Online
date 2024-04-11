using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_CreateMapHandler(PacketSession session, IMessage packet)
    {
        C_CreateMap createPacket = packet as C_CreateMap;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.MyPlayer == null)
        {
            return;
        }
        if (clientSession.MyPlayer.Room == null)
        {
            return;
        }

        // TODO : 검증

        // 다른 플레이어한테도 알려준다
        S_CreateMap rescreatePacket = new S_CreateMap();
        rescreatePacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
        foreach (NodeInfo nodeInfo in createPacket.NodeInfo)
        {
            rescreatePacket.NodeInfo.Add(nodeInfo);
        }

        clientSession.MyPlayer.Room.Broadcast(rescreatePacket);
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.MyPlayer == null)
        {
            return;
        }
        if (clientSession.MyPlayer.Room == null)
        {
            return;
        }

        // TODO : 검증

        // 다른 플레이어한테도 알려준다
        S_Move resMovePacket = new S_Move();
        resMovePacket.PlayerId = clientSession.MyPlayer.Info.PlayerId;
        resMovePacket.IsStart = movePacket.IsStart;
        resMovePacket.PosInfo = movePacket.PosInfo;

        clientSession.MyPlayer.Room.Broadcast(resMovePacket);
    }
}

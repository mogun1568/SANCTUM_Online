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
    public static void C_GameStartHandler(PacketSession session, IMessage packet)
    {
        C_GameStart gameStartPacket = packet as C_GameStart;
        ClientSession clientSession = session as ClientSession;

        // 멀티쓰레드 환경에서는 실행 중간에 바뀔 수 있으므로 따로 빼와서 확인하는 것이 좋다
        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        GameRoom room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleGameStart, player, gameStartPacket);
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        GameRoom room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_CreateTurretHandler(PacketSession session, IMessage packet)
    {
        C_CreateTurret createTurretPacket = packet as C_CreateTurret;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        GameRoom room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleCreateTurret, player, createTurretPacket);
    }
}

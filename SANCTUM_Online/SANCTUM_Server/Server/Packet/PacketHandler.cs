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
        C_CreateMap createMapPacket = packet as C_CreateMap;
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

        room.HandleCreateMap(player, createMapPacket);
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

        room.HandleMove(player, movePacket);
    }

    public static void C_SpawnEnemyHandler(PacketSession session, IMessage packet)
    {
        C_SpawnEnemy spawnEnemyPacket = packet as C_SpawnEnemy;
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

        room.HandleSpawnEnemy(player, spawnEnemyPacket);
    }
}

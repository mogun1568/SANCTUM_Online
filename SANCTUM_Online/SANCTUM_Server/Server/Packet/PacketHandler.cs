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
    public static void C_RoomListHandler(PacketSession session, IMessage packet)
    {
        C_RoomList roomListPacket = packet as C_RoomList;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        WaitingRoom room = player.WaitingRoom;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleRoomList, player, roomListPacket);
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterRoomPacket = packet as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        WaitingRoom room = player.WaitingRoom;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleEnterRoom, player, enterRoomPacket);
    }

    public static void C_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        C_LeaveRoom leaveRoomPacket = packet as C_LeaveRoom;
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

        room.Push(room.HandleLeaveRoom, player, leaveRoomPacket);
    }

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

    public static void C_InvenUpdateHandler(PacketSession session, IMessage packet)
    {
        C_InvenUpdate invenUpdatePacket = packet as C_InvenUpdate;
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

        room.Push(room.HandleInvenUpdate, player, invenUpdatePacket);
    }

    public static void C_LevelUpHandler(PacketSession session, IMessage packet)
    {
        C_LevelUp levelUpPacket = packet as C_LevelUp;
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

        room.Push(room.HandlelevelUp, player, levelUpPacket);
    }

    public static void C_TurretUIHandler(PacketSession session, IMessage packet)
    {
        C_TurretUI turretUIPacket = packet as C_TurretUI;
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

        room.Push(room.HandleTurretUI, player, turretUIPacket);
    }

    public static void C_TurretDemoliteHandler(PacketSession session, IMessage packet)
    {
        C_TurretDemolite turretDemolitePacket = packet as C_TurretDemolite;
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

        room.Push(room.HandleTurretDemolite, player, turretDemolitePacket);
    }

    public static void C_FirstPersonModeHandler(PacketSession session, IMessage packet)
    {
        C_FirstPersonMode firstPersonModePacket = packet as C_FirstPersonMode;
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

        room.Push(room.HandleFirstPersonMode, player, firstPersonModePacket);
    }

    public static void C_ShootHandler(PacketSession session, IMessage packet)
    {
        C_Shoot shootPacket = packet as C_Shoot;
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

        room.Push(room.HandleShoot, player, shootPacket);
    }

    public static void C_PauseHandler(PacketSession session, IMessage packet)
    {
        C_Pause pausePacket = packet as C_Pause;
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

        room.Push(room.HandlePause, player, pausePacket);
    }
}

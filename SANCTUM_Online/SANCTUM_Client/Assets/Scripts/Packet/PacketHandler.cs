using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, myMap: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Managers.Object.RemoveMyPlayer();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (PlayerInfo player in spawnPacket.Players)
        {
            Managers.Object.Add(player, myMap: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.PlayerIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_CreateMapHandler(PacketSession session, IMessage packet)
    {
        S_CreateMap CreatePacket = packet as S_CreateMap;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(CreatePacket.PlayerId);
        if (go == null)
        {
            return;
        }

        Debug.Log($"{CreatePacket.PlayerId}, {Managers.Object.MyMap.Id}");
        if (CreatePacket.PlayerId == Managers.Object.MyMap.Id)
        {
            return;
        }


        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        // 자기 자신은 클라에서 이동시키므로 굳이 이렇게 콜백을 받을 필요는 없음
        foreach (NodeInfo nodeInfo in CreatePacket.NodeInfo)
        {
            mc.CreateNode(nodeInfo.NodeType, nodeInfo.PosInfo.PosX, nodeInfo.PosInfo.PosZ);
        }
    }
}

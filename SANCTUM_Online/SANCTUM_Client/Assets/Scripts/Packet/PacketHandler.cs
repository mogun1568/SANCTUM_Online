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
        S_CreateMap createPacket = packet as S_CreateMap;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(createPacket.PlayerId);
        if (go == null)
        {
            return;
        }

        if (createPacket.PlayerId == Managers.Object.MyMap.Id)
        {
            return;
        }


        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        // 자기 자신은 클라에서 이동시키므로 굳이 이렇게 콜백을 받을 필요는 없음
        foreach (NodeInfo nodeInfo in createPacket.NodeInfo)
        {
            mc.CreateNode(nodeInfo.NodeType, nodeInfo.PosInfo.PosX, nodeInfo.PosInfo.PosZ, nodeInfo.HaveEnvironment);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(movePacket.PlayerId);
        if (go == null)
        {
            return;
        }

        if (movePacket.PlayerId == Managers.Object.MyMap.Id)
        {
            return;
        }

        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        if (movePacket.IsStart)
        {
            // 자기 자신은 클라에서 이동시키므로 굳이 이렇게 콜백을 받을 필요는 없음
            mc.startPoint = new LocationInfo(movePacket.PosInfo.PosX, movePacket.PosInfo.PosZ, movePacket.PosInfo.Dir);
        }
        else
        {
            mc.endPoint = new LocationInfo(movePacket.PosInfo.PosX, movePacket.PosInfo.PosZ, movePacket.PosInfo.Dir);
        }

        mc.UpdatePosition();
    }

    public static void S_SpawnEnemyHandler(PacketSession session, IMessage packet)
    {
        S_SpawnEnemy spawnEnemyPacket = packet as S_SpawnEnemy;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(spawnEnemyPacket.PlayerId);
        if (go == null)
        {
            return;
        }

        if (spawnEnemyPacket.PlayerId == Managers.Object.MyMap.Id)
        {
            return;
        }

        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        GameObject monster = Managers.Resource.Instantiate($"Monster/{spawnEnemyPacket.EnemyName}", mc.startObj.transform.position, mc.startObj.transform.rotation);
        NewMap map = Managers.Object.FindById(spawnEnemyPacket.PlayerId).GetComponent<NewMap>();
        monster.GetComponent<EnemyMovement>().nextRoad = map.roads.First.Next;
        monster.GetComponent<EnemyMovement>().mapId = map.Id;
    }
}

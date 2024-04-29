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

    public static void S_GameStartHandler(PacketSession session, IMessage packet)
    {
        S_GameStart gameStartPacket = packet as S_GameStart;

        GameObject go = Managers.Object.FindById(gameStartPacket.PlayerId);
        if (go == null)
        {
            return;
        }

        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        Managers.Game.GameStartFlag = true;
    }

    public static void S_CreateMapHandler(PacketSession session, IMessage packet)
    {
        S_CreateMap createMapPacket = packet as S_CreateMap;

        GameObject go = Managers.Object.FindById(createMapPacket.PlayerId);
        if (go == null)
        {
            return;
        }

        NewMap mc = go.GetComponent<NewMap>();
        if (mc == null)
        {
            return;
        }

        // 자기 자신은 클라에서 이동시키므로 굳이 이렇게 콜백을 받을 필요는 없음
        mc.LoadMap(createMapPacket.PlayerId);
        foreach (NodeInfo nodeInfo in createMapPacket.NodeInfo)
        {
            mc.CreateNode(nodeInfo);
        }

        //if (createMapPacket.PlayerId == Managers.Object.MyMap.Id)
        //{
        //    Debug.Log("me");
        //    foreach (int key in Managers.Object._grounds.Keys)
        //    {
        //        //GameObject value = Managers.Object._grounds[key];
        //        Debug.Log($"Key: {key}");
        //    }
        //}

        mc.UpdatePosition();
    }

    public static void S_SpawnEnemyHandler(PacketSession session, IMessage packet)
    {
        S_SpawnEnemy spawnEnemyPacket = packet as S_SpawnEnemy;

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

    public static void S_CreateTurretHandler(PacketSession session, IMessage packet)
    {
        S_CreateTurret createTurretPacket = packet as S_CreateTurret;

        GameObject go = Managers.Object.FindByNodeId(createTurretPacket.NodeId);
        if (go == null)
        {
            return;
        }

        Node n = go.GetComponent<Node>();
        if (n == null)
        {
            return;
        }

        n.GetComponent<Node>().BuildTurret(createTurretPacket.PlayerId, createTurretPacket.ItemName);
    }
}

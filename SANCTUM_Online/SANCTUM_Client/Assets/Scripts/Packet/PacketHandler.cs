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
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myMap: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.ObjectIds)
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
        mc.UpdatePosition();
    }

    public static void S_LookHandler(PacketSession session, IMessage packet)
    {
        S_Look lookPacket = packet as S_Look;

        GameObject go = Managers.Object.FindById(lookPacket.ObjectId);
        if (go == null)
        {
            return;
        }

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc._targetPos = lookPacket.TargetPosinfo;
        bc.State = CreatureState.Attacking;
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
        {
            return;
        }

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc.PosInfo = movePacket.PosInfo;
        bc.State = CreatureState.Moving;
    }

    public static void S_CreateTurretHandler(PacketSession session, IMessage packet)
    {
        S_CreateTurret createTurretPacket = packet as S_CreateTurret;

        GameObject go = Managers.Object.FindById(createTurretPacket.NodeId);
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

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;

        GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc.Hp = changePacket.Hp;
        Debug.Log(bc.Hp);
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc.Hp = 0;
        bc.OnDead();
    }
}

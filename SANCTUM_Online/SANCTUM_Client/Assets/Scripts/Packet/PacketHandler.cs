using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;

        if (enterGamePacket.IsGameRoom)
            Managers.Object.Add(enterGamePacket.Player, myMap: true);
        else
            Managers.Object.roomAdd(enterGamePacket.Player, myMap: true);
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
            Managers.Object.MyRoom.exit(id);
            Managers.Object.Remove(id);
        }
    }

    public static void S_RoomListHandler(PacketSession session, IMessage packet)
    {
        S_RoomList roomListPacket = packet as S_RoomList;
        foreach (RoomInfo ri in roomListPacket.RoomList)
        {
            Managers.Object.RoomList.UpdateRoomList(ri);
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

        Managers.UI.ClosePopupUI();
        Managers.UI.ShowSceneUI<UI_Scene>("MainUI");
        GameObject invenUI = Managers.UI.ShowSceneUI<UI_Inven>("InvenUI").gameObject;
        Managers.Game.invenUI = invenUI;
        Managers.UI.SelectItem.LoadInventory(gameStartPacket.PlayerId);
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

        //if (Managers.Object.MyMap.Id == movePacket.ObjectId)
        //    return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc.PosInfo = movePacket.PosInfo;
        bc.State = CreatureState.Moving;
    }

    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat changeStatPacket = packet as S_ChangeStat;

        GameObject go = Managers.Object.FindById(changeStatPacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            return;
        }

        bc.Stat = changeStatPacket.StatInfo;
        //Debug.Log(bc.Hp);
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

    public static void S_InvenUpdateHandler(PacketSession session, IMessage packet)
    {
        S_InvenUpdate invenUpdatePacket = packet as S_InvenUpdate;

        if (invenUpdatePacket.PlayerId != Managers.Object.MyMap.Id)
            return;

        Managers.UI.SelectItem.LoadInventory(invenUpdatePacket.PlayerId);
    }

    public static void S_ExpUpdateHandler(PacketSession session, IMessage packet)
    {
        S_ExpUpdate expUpdatePacket = packet as S_ExpUpdate;

        MyMapController myMap = Managers.Object.MyMap;
        if (expUpdatePacket.PlayerId != myMap.Id)
            return;

        myMap.Stat.Exp = expUpdatePacket.Exp;
        myMap.Stat.TotalExp = expUpdatePacket.TotalExp;
        myMap._countLevelUp += expUpdatePacket.CountLevelUp;
    }

    public static void S_TurretUIHandler(PacketSession session, IMessage packet)
    {
        S_TurretUI turretUIPacket = packet as S_TurretUI;

        if (turretUIPacket.PlayerId != Managers.Object.MyMap.Id)
            return;

        GameObject node = Managers.Object.FindById(turretUIPacket.NodeId);
        Node nc = node.GetComponent<Node>();

        GameObject turret = Managers.Object.FindById(turretUIPacket.TurretId);
        Turret tc = turret.GetComponent<Turret>();

        NodeUI nodeUI = Managers.UI.ShowPopupUI<NodeUI>("NodeUI");
        nodeUI.SetTarget(nc, tc);
    }

    public static void S_FirstPersonModeHandler(PacketSession session, IMessage packet)
    {
        S_FirstPersonMode firstPersonModePacket = packet as S_FirstPersonMode;

        if (firstPersonModePacket.PlayerId != Managers.Object.MyMap.Id)
            return;

        Managers.Object.MyMap.IsFPM = false;
        Managers.UI.ClosePopupUI();
        Managers.Game.invenUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Managers.Object.MyMap._mainCamera.gameObject.SetActive(true);

        Managers.Object.MyMap.StartlevelUpCoroutine();
    }
}

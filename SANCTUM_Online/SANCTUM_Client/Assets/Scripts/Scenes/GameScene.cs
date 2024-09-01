using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameScene : BaseScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.MultiPlay;

        C_EnterRoom enterRoomPacket = new C_EnterRoom();
        enterRoomPacket.RoomId = Managers.Object.RoomList.RoomId;
        enterRoomPacket.IsGameRoom = true;
        Managers.Object.RoomList = null;
        Managers.Network.Send(enterRoomPacket);

        Managers.Game._mainCamera = Camera.main;

        //Managers.Game.Init();
        //Managers.Scene.Init();
    }

    void Update()
    {
        if (Managers.Game.GameIsOver)
        {
            return;
        }

        if (Managers.Scene.sceneFader.isFading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.Game.Toggle();
        }
    }

    public override void Clear()
    {
        Managers.Game.Init();
    }
}

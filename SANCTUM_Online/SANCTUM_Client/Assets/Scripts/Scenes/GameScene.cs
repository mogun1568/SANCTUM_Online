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

        //Screen.SetResolution(640, 480, false);

        C_EnterRoom enterRoomPacket = new C_EnterRoom();
        enterRoomPacket.RoomId = Managers.Object.RoomList.RoomId;
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

        if (Input.GetKeyDown(KeyCode.Escape)) // Input.GetKeyDown(KeyCode.P)
        {
            Managers.Game.Toggle();
        }

        Managers.Game.gameTime += Time.deltaTime;
    }

    public override void Clear()
    {
        Managers.Game.Init();
    }
}

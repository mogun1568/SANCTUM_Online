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

        //Managers.UI.ShowSceneUI<UI_Scene>("MainUI");
        //GameObject invenUI = Managers.UI.ShowSceneUI<UI_Inven>("InvenUI").gameObject;
        //Managers.Game.invenUI = invenUI;

        //Managers.UI.getPopStackTop().GetComponentInChildren<SelectItem>().FirstAddItem();
        //GameObject.Find("InvenUI").GetComponentInChildren<SelectItem>().FirstAddItem();


        //Managers.UI.ShowSceneUI<UI_Inven>();

        //Dictionary<string, Data.Item> dict = Managers.Data.ItemDict;
        //Debug.Log("me");

        // Pool 관련 코드
        /*for (int i = 0; i < 5; i++)
        {
            Managers.Resource.Instantiate("UnityChan");
        }*/

        // 코루틴 관련 코드
        /*co = StartCoroutine("ExplodeAfterSeconds", 4.0f);
        StartCoroutine("CoStopExplode", 2.0f);*/
    }

    void Update()
    {
        // 확인용 (나중에 고쳐야됨 문제 많음)
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (Managers.Object._objects.Count >= 1 && !Managers.Game.GameStartFlag)
        //    {
        //        C_GameStart gameStartPacket = new C_GameStart();
        //        Managers.Network.Send(gameStartPacket);
        //    }
        //}
        //if (!Managers.Game.GameStartFlag)
        //{
        //    return;
        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    if (Managers.Object._objects.Count >= 2 && !check)
        //    {
        //        Managers.Object.MyMap.GameStart();
        //        check = true;
        //    }
        //    else
        //    {
        //        Debug.Log("No more than 1 player.");
        //    }
        //}


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

        //if (!Managers.Game.isLive)
        //{
        //    return;
        //}

        /*if (!Managers.Game.isFPM && Managers.UI.getPopStackTop()?.name == "FPSModeUI")
        {
            Managers.UI.ClosePopupUI();
            Debug.Log(Managers.Game.countLevelUp);
        }*/

        //Debug.Log($"{Managers.Object.MyMap._countLevelUp}, {Managers.Game.isFPM}");
        //if (Managers.Object.MyMap._countLevelUp > 0 && !Managers.Game.isFPM && !Managers.Game.isPractice)
        //{
        //    Managers.Game.isPractice = true;
        //    Debug.Log(Managers.Object.MyMap._countLevelUp);
        //    StartCoroutine(Managers.Game.WaitForItemSelection(Managers.Object.MyMap._countLevelUp));
        //    Managers.Object.MyMap._countLevelUp = 0;
        //}



        Managers.Game.gameTime += Time.deltaTime;
    }

    public override void Clear()
    {
        Managers.Game.Init();
    }
}

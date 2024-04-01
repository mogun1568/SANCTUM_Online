using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class GameScene : BaseScene
{
    //Coroutine co;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.GamePlay;

        //Managers.Game.Init();
        //Managers.Scene.Init();
        Managers.UI.ShowSceneUI<UI_Scene>("MainUI");
        GameObject invenUI = Managers.UI.ShowSceneUI<UI_Inven>("InvenUI").gameObject;
        Managers.Game.invenUI = invenUI;

        //Managers.UI.getPopStackTop().GetComponentInChildren<SelectItem>().FirstAddItem();
        //GameObject.Find("InvenUI").GetComponentInChildren<SelectItem>().FirstAddItem();


        //Managers.UI.ShowSceneUI<UI_Inven>();

        //Dictionary<string, Data.Item> dict = Managers.Data.ItemDict;
        //Debug.Log("me");

        // Pool 包访 内靛
        /*for (int i = 0; i < 5; i++)
        {
            Managers.Resource.Instantiate("UnityChan");
        }*/

        // 内风凭 包访 内靛
        /*co = StartCoroutine("ExplodeAfterSeconds", 4.0f);
        StartCoroutine("CoStopExplode", 2.0f);*/
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

        if (!Managers.Game.isLive)
        {
            return;
        }

        if (Managers.Game.Lives <= 0)
        {
            Managers.Game.EndGame();
        }

        /*if (!Managers.Game.isFPM && Managers.UI.getPopStackTop()?.name == "FPSModeUI")
        {
            Managers.UI.ClosePopupUI();
            Debug.Log(Managers.Game.countLevelUp);
        }*/

        if (Managers.Game.countLevelUp > 0 && !Managers.Game.isFPM)
        {
            StartCoroutine(Managers.Game.WaitForItemSelection());
        }
         
        

        Managers.Game.gameTime += Time.deltaTime;
    }

    public override void Clear()
    {
        Managers.Game.Init();
    }
}

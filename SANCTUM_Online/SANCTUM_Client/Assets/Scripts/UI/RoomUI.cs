using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomUI : UI_Popup
{
    enum GameObjects
    {
        Character
    }

    enum Buttons
    {
        Button_Back,
        Button_Home,
        Button_Start
    }

    GameObject Character;

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        Character = GetObject((int)GameObjects.Character);

        BindEvent(GetButton((int)Buttons.Button_Back).gameObject, (PointerEventData data) => { Home(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Home).gameObject, (PointerEventData data) => { Home(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Start).gameObject, (PointerEventData data) => { GameStart(); }, Define.UIEvent.Click);
    }

    public void Home()
    {
        Managers.Game.Toggle();
        Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
    }

    public void GameStart()
    {
        if (Character.transform.childCount >= 1 && !Managers.Game.GameStartFlag)
        {
            C_GameStart gameStartPacket = new C_GameStart();
            Managers.Network.Send(gameStartPacket);
        }
    }

    public void Join(string name, bool myRoom)
    {
        GameObject go;
        if (myRoom)
            go = Character.transform.GetChild(0).gameObject;
        else
            go = Instantiate(Character.transform.GetChild(0).gameObject, Character.transform);

        go.name = name;
        go.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    public void exit(int id)
    {
        string name = $"Player_{id}";

        GameObject go = FindChildByName(name);
        if (go == null)
            return;

        Destroy(go);
    }

    public GameObject FindChildByName(string childName)
    {
        foreach (Transform child in Character.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}

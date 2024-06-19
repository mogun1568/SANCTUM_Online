using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomSelectUI : UI_Scene
{
    enum GameObjects
    {
        Content
    }

    enum Buttons
    {
        Button_Back,
        Button_CreateRoom,
        Button_Update
    }

    GameObject Content;
    public int RoomId { get; set; }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        Content = GetObject((int)GameObjects.Content);

        BindEvent(GetButton((int)Buttons.Button_Back).gameObject, (PointerEventData data) => { Home(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_CreateRoom).gameObject, (PointerEventData data) => { CreateOrJoinRoom(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Update).gameObject, (PointerEventData data) => { RequestRoomList(); }, Define.UIEvent.Click);
    }

    public void Home()
    {
        Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
    }

    public void CreateOrJoinRoom(int roomId = default)
    {
        if (roomId != default)
            RoomId = roomId;

        Managers.Scene.sceneFader.FadeTo(Define.Scene.MultiPlay);
    }

    public void RequestRoomList()
    {
        Content.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 1; i < Content.transform.childCount; i++)
            Destroy(Content.transform.GetChild(i).gameObject);

        C_RoomList roomListPacket = new C_RoomList();
        Managers.Network.Send(roomListPacket);
    }

    public void UpdateRoomList(RoomInfo roomInfo)
    {
        GameObject go;
        if (Content.transform.childCount == 1)
        {
            go = Content.transform.GetChild(0).gameObject;
            go.SetActive(true);
        }
        else
            go = Instantiate(Content.transform.GetChild(0).gameObject, Content.transform);

        go.GetComponentsInChildren<TextMeshProUGUI>()[0].text = $"Room_{roomInfo.Id}";
        go.GetComponentsInChildren<TextMeshProUGUI>()[2].text = $"{roomInfo.PlayerNum} / 4";

        Button button = go.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); // 기존의 리스너를 모두 제거 (중복 방지)
            button.onClick.AddListener(() => CreateOrJoinRoom(roomInfo.Id)); // 새로운 리스너 등록
        }
    }

    public GameObject FindChildByName(string childName)
    {
        foreach (Transform child in Content.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}

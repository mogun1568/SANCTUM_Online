using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScene : BaseScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Managers.Network.Init();
        SceneType = Define.Scene.Room;

        Managers.Object.RoomList = Managers.UI.ShowSceneUI<RoomSelectUI>("RoomSelectUI");
    }

    public override void Clear()
    {
        
    }
}

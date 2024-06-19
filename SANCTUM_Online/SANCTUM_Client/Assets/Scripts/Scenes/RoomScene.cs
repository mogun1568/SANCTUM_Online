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
    }

    public override void Clear()
    {
        
    }
}

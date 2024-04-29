using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class MyMapController : NewMap
{
    protected override void Init()
    {
        base.Init();

        int cameraDefault = MapDefaultSize * 2 - 24;
        Camera.main.transform.position = Pos + new Vector3(cameraDefault, 40, cameraDefault);
    }

    public override void ExpendMap()
    {
        CheckUpdatedMap();
    }

    void CheckUpdatedMap()
    {
        C_CreateMap createMapPacket = new C_CreateMap();

        Managers.Network.Send(createMapPacket);
    }
}

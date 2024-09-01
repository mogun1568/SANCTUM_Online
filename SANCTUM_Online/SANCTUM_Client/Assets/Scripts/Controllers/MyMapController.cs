using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;

public class MyMapController : NewMap
{
    //public Camera _mainCamera;

    protected override void Init()
    {
        base.Init();

        //_mainCamera = Camera.main;

        int cameraDefault = MapDefaultSize * 2 - 24;
        //_mainCamera.transform.position = Pos + new Vector3(cameraDefault, 40, cameraDefault);
    }

    public override void Update()
    {

    }
}

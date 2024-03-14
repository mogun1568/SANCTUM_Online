using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeScene : BaseScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.GamePlay;

        
    }

    void Update()
    {
        
    }

    public override void Clear()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.MainMenu;

        Managers.UI.ShowSceneUI<MainMenu>("MainMenuUI");
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
}

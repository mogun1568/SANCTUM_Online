using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : UI_Scene
{
    enum Buttons {
        PlayButton,
        SettingButton,
        QuitButton
    }

    void Awake()
    {
        base.Init();

        Managers.Sound.Play("Bgms/bgm2", Define.Sound.Bgm);

        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.PlayButton).gameObject, (PointerEventData data) => { Play(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, (PointerEventData data) => { Setting(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.QuitButton).gameObject, (PointerEventData data) => { Quit(); }, Define.UIEvent.Click);
    }

    public void Play()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Managers.Scene.sceneFader.FadeTo(Define.Scene.Room);
    }

    public void Setting()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Managers.UI.ShowPopupUI<Setting>("SettingUI");
    }

    public void Quit()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Debug.Log("Exciting...");
        Application.Quit();
    }
}

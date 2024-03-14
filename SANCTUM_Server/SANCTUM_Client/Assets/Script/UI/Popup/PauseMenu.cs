using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : UI_Popup
{
    //string MainToLoad = "MainMenu";

    enum Buttons
    {
        Button_Setting,
        Button_Continue,
        Button_Retry,
        Button_GoToMenu
    }

    void Awake()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.Button_Setting).gameObject, (PointerEventData data) => { Setting(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Continue).gameObject, (PointerEventData data) => { Managers.Game.Toggle(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Retry).gameObject, (PointerEventData data) => { Retry(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_GoToMenu).gameObject, (PointerEventData data) => { Menu(); }, Define.UIEvent.Click);
    }

    public void Retry()
    {
        Managers.Game.Toggle();
        Managers.Scene.sceneFader.FadeTo(Define.Scene.GamePlay);
    }

    public void Menu()
    {
        Managers.Game.Toggle();
        Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
    }

    public void Setting()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Managers.UI.ShowPopupUI<Setting>("SettingUI");
    }
}

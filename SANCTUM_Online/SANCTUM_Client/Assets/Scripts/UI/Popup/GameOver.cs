using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOver : UI_Popup
{
    //public string MainToLoad = "MainMenu";

    //public SceneFader sceneFader;

    enum Buttons
    {
        RetryButton,
        Button_Exit
    }

    enum Texts
    {
        Rounds
    }

    void Awake()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Managers.Sound.Play("Bgms/cinematic-melody-main-9785", Define.Sound.Bgm);
        BindEvent(GetButton((int)Buttons.RetryButton).gameObject, (PointerEventData data) => { Retry(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.Button_Exit).gameObject, (PointerEventData data) => { Menu(); }, Define.UIEvent.Click);
        GetText((int)Texts.Rounds).text = Managers.Game.Rounds.ToString(); ;
    }

    public void Retry()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Managers.Game.Resume();
        Managers.Scene.sceneFader.FadeTo(Define.Scene.GamePlay);
    }

    public void Menu()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        Managers.Game.Resume();
        Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
    }
}

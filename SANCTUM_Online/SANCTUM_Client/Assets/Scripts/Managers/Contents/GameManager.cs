using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    [Header("# Game Control")]
    //public bool isFPM;
    public float gameTime;
    public bool GameIsOver;
    public bool GameStartFlag;

    public Camera _mainCamera;
    public GameObject invenUI;

    public void Init()
    {
        //isFPM = false;
        GameIsOver = false;
        GameStartFlag = false;
        gameTime = 0;
    }

    // PauseMenu로 이전할 듯?
    public bool isPopup = false, isSettingUI = false;
    public void Toggle()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        
        if (isSettingUI)
        {
            Managers.UI.getPopStackTop().GetComponent<Setting>().Close();
            return;
        }
        
        C_Pause pausePacket = new C_Pause();

        if (!isPopup)
        {
            Managers.UI.ShowPopupUI<PauseMenu>("PauseMenuUI(Multy)");
            isPopup = true;

            if (Managers.Object.MyMap.IsFPM)
                Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Managers.UI.ClosePopupUI();
            isPopup = false;

            if (Managers.Object.MyMap.IsFPM)
                Cursor.lockState = CursorLockMode.Locked;
        }

        pausePacket.IsPause = isPopup;
        Managers.Network.Send(pausePacket);
    }

    //public void Stop()
    //{
    //    isLive = false;
    //    Time.timeScale = 0;
    //    Cursor.lockState = CursorLockMode.None;
    //}

    //public void Resume()
    //{
    //    isLive = true;
    //    Time.timeScale = 1;

    //    if (isFPM)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //    }
    //}
}

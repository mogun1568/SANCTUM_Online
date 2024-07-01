using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    [Header("# Game Control")]
    // 이거 제대로 쓰려면 모든 스크립트 Update 함수에 isLive 참일 때만 돌아가게 해야함 - 했음
    public bool isLive;
    //public bool isFPM;
    public float gameTime;
    public bool GameIsOver;
    public bool GameStartFlag;

    public Camera _mainCamera;
    public GameObject invenUI;

    public void Init()
    {
        isLive = true;
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

        if (!isPopup)
        {
            Managers.UI.ShowPopupUI<PauseMenu>("PauseMenuUI(Multy)");
            isPopup = true;
        }
        else
        {
            Managers.UI.ClosePopupUI();
            isPopup = false;
        }
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

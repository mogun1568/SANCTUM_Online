using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    [Header("# Game Control")]
    // �̰� ����� ������ ��� ��ũ��Ʈ Update �Լ��� isLive ���� ���� ���ư��� �ؾ��� - ����
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

    // PauseMenu�� ������ ��?
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

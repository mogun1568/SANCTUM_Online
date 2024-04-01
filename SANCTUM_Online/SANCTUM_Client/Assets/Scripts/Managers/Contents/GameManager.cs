using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    //public static GameManager instance;

    [Header("# Game Control")]
    // 이거 제대로 쓰려면 모든 스크립트 Update 함수에 isLive 참일 때만 돌아가게 해야함 - 했음
    public bool isLive;
    public bool isFPM;
    public float gameTime;
    public bool GameIsOver;

    [Header("# Player Info")]
    public int Lives;
    public int startLives = 10;
    public int Rounds;
    public int exp;
    public int nextExp = 3;
    public int countLevelUp;
    [HideInInspector] public bool isHide;

    public GameObject invenUI;

    //[Header("# Game Object")]
    //public PoolManager pool;
    //public Map map;
    //public GameObject gameOverUI;
    //public LevelUp uiLevelUp;
    //public GameObject MainCamera;
    //public SoundManager soundManager;

    public void Init()
    {
        isLive = true;
        isFPM = false;
        GameIsOver = false;
        gameTime = 0;
        Lives = startLives;
        Rounds = 0;
        exp = 0;
        nextExp = 3;
        countLevelUp = 0;

        //Debug.Log(nextExp);
    }

    /*void Awake()
    {
        instance = this;
    }*/

    /*void Update()
    {
        if (GameIsOver)
        {
            return;
        }

        if (!isLive)
        {
            return;
        }

        if (Lives <= 0)
        {
            EndGame();
        }

        gameTime += Time.deltaTime;
    }*/

    public void EndGame()
    {
        GameIsOver = true;
        //gameOverUI.SetActive(true);
        Managers.UI.CloseAllPopupUI();
        Managers.UI.ShowPopupUI<GameOver>("GameOverUI");
        Stop();
    }

    public void GetExp(int _exp)
    {
        exp += _exp;

        while (exp >= nextExp)
        {
            //nextExp *= 1;
            exp -= nextExp;
            nextExp = Mathf.RoundToInt(nextExp * 1.5f);
            Mathf.Clamp(nextExp, 0, 10);
            countLevelUp++;
            //Debug.Log(countLevelUp);
        }

        /*if (countLevelUp > 0 && !isFPM)
        {
            StartCoroutine(WaitForItemSelection());
        }*/
    }

    public IEnumerator WaitForItemSelection()
    {
        while (countLevelUp > 0)
        {
            if (Managers.UI.getPopStackTop()?.name == "NodeUI")
            {
                Managers.UI.ClosePopupUI();
            }
            Managers.UI.ShowPopupUI<LevelUp>("LevelUpUI");
            //ShowUI.Show();
            //uiLevelUp.Show();
            countLevelUp--;
            // Hide() 함수가 실행되면 넘어가도록 해야됨
            isHide = false;
            yield return new WaitUntil(() => isHide);
        }
    }

    public bool isPopup = false, isSettingUI = false;
    public void Toggle()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);
        //GameManager.instance.soundManager.Play("Effects/UiClickLow", SoundManager.Sound.Effect);

        if (isSettingUI)
        {
            Managers.UI.getPopStackTop().GetComponent<Setting>().Close();
            //Managers.UI.ClosePopupUI();
            //settingUI.SetActive(false);
            return;
        }

        //ui.SetActive(!ui.activeSelf);

        if (!isPopup)
        {
            Managers.UI.ShowPopupUI<PauseMenu>("PauseMenuUI");
            isPopup = true;
            Stop();
        }
        else
        {
            Managers.UI.ClosePopupUI();
            isPopup = false;
            Resume();
            //GameManager.instance.soundManager.Play("Effects/UiClickLow", SoundManager.Sound.Effect);
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;

        if (isFPM)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
